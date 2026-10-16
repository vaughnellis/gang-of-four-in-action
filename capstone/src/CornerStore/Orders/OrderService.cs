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
        return order.State.Pay(this, order);
    }

    public string? Ship(Order order)
    {
        return order.State.Ship(this, order);
    }

    public string? Deliver(Order order)
    {
        return order.State.Deliver(this, order);
    }

    public string? Cancel(Order order)
    {
        return order.State.Cancel(this, order);
    }

    public string? Refund(Order order)
    {
        return order.State.Refund(this, order);
    }

    internal void RecordStatusChange(Order order)
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

    internal void RestoreStock(Order order)
    {
        foreach (var line in order.Lines)
        {
            inventory.AdjustStock(line.Product.Sku, line.Quantity);
        }

        order.StockReserved = false;
    }

    internal void AssignTrackingNumber(Order order)
    {
        if (order.TrackingNumber is null)
        {
            order.TrackingNumber = ids.NextShipmentId();
        }
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