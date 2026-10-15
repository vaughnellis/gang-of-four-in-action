namespace CornerStore;

internal class OrderService(
    IdSequence ids,
    InventoryService inventory,
    OrderConfirmedEmail confirmedEmail,
    OrderConfirmedSms confirmedSms,
    OrderShippedEmail shippedEmail,
    OrderShippedSms shippedSms,
    LoyaltyService loyalty,
    AnalyticsService analytics)
{
    private readonly Dictionary<string, Order> orders = new(StringComparer.Ordinal);
    private readonly List<Invoice> invoices = [];

    public Order Create(Customer customer, Cart cart, Quote quote, string paymentType)
    {
        var order = new OrderBuilder()
            .WithId(ids.NextOrderId())
            .WithCustomer(customer)
            .WithLines(cart.Lines.Select(CopyLine).ToList())
            .WithQuote(quote)
            .WithPaymentType(paymentType)
            .WithPromoCode(cart.PromoCode == "" ? null : cart.PromoCode)
            .Build();
        orders[order.Id] = order;
        CreateInvoice(order);
        return order;
    }

    public Invoice CreateInvoice(Order order)
    {
        var invoice = new InvoiceBuilder()
            .ForOrder(order)
            .Build();
        invoices.Add(invoice);
        return invoice;
    }

    public Order? Find(string id)
    {
        return orders.GetValueOrDefault(id);
    }

    public List<Order> GetAll()
    {
        return orders.Values.OrderBy(order => order.Id, StringComparer.Ordinal).ToList();
    }

    public string? Pay(Order order)
    {
        // SMELL S20: the allowed transition is selected by comparing the status string
        if (order.Status != "Pending")
        {
            return "cannot pay from " + order.Status;
        }

        order.Status = "Paid";
        RecordStatusChange(order);
        return null;
    }

    public string? Ship(Order order)
    {
        // SMELL S20: the allowed transition is selected by comparing the status string
        if (order.Status != "Paid")
        {
            return "cannot ship from " + order.Status;
        }

        order.Status = "Shipped";
        if (order.TrackingNumber is null)
        {
            order.TrackingNumber = ids.NextShipmentId();
        }

        RecordStatusChange(order);
        return null;
    }

    public string? Deliver(Order order)
    {
        // SMELL S20: the allowed transition is selected by comparing the status string
        if (order.Status != "Shipped")
        {
            return "cannot deliver from " + order.Status;
        }

        order.Status = "Delivered";
        RecordStatusChange(order);
        return null;
    }

    public string? Cancel(Order order)
    {
        // SMELL S20: the allowed transition is selected by comparing the status string
        if (order.Status != "Pending" && order.Status != "Paid")
        {
            return "cannot cancel from " + order.Status;
        }

        order.Status = "Cancelled";
        if (order.StockReserved)
        {
            RestoreStock(order);
        }

        RecordStatusChange(order);
        return null;
    }

    public string? Refund(Order order)
    {
        // SMELL S20: the allowed transition is selected by comparing the status string
        if (order.Status != "Paid" && order.Status != "Shipped" && order.Status != "Delivered")
        {
            return "cannot refund from " + order.Status;
        }

        order.Status = "Refunded";
        if (order.StockReserved)
        {
            RestoreStock(order);
        }

        RecordStatusChange(order);
        return null;
    }

    private void RecordStatusChange(Order order)
    {
        // SMELL S19: order transitions directly call stock, notification, loyalty and analytics services
        foreach (var line in order.Lines)
        {
            inventory.GetStock(line.Product.Sku);
        }

        if (order.Status == "Paid")
        {
            if (order.Customer.Channel == "email")
            {
                confirmedEmail.Send(order.Customer, order.Id, order.Quote.Total);
            }
            else
            {
                confirmedSms.Send(order.Customer, order.Id, order.Quote.Total);
            }
        }
        else if (order.Status == "Shipped")
        {
            if (order.Customer.Channel == "email")
            {
                shippedEmail.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
            }
            else
            {
                shippedSms.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
            }
        }

        loyalty.RecordStatusChange(order);
        analytics.RecordStatusChange(order);
    }

    private void RestoreStock(Order order)
    {
        foreach (var line in order.Lines)
        {
            inventory.AdjustStock(line.Product.Sku, line.Quantity);
        }

        order.StockReserved = false;
    }

    private static CartLine CopyLine(CartLine line)
    {
        return new CartLine
        {
            Product = line.Product,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            FromBundle = line.FromBundle,
        };
    }
}