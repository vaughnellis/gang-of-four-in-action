namespace CornerStore;

internal class OrderService(
    IdSequence ids,
    InventoryService inventory,
    IChannel emailChannel,
    IChannel smsChannel,
    LoyaltyService loyalty,
    AnalyticsService analytics)
{
    private readonly Dictionary<string, Order> orders = new(StringComparer.Ordinal);
    private readonly List<Invoice> invoices = [];

    public Order Create(Customer customer, Cart cart, Quote quote, string paymentType)
    {
        // SMELL S02: the order is assembled with eleven positional arguments and null optionals
        var order = new Order(
            ids.NextOrderId(), customer, "Pending",
            cart.Lines.Select(CopyLine).ToList(), quote, paymentType,
            cart.PromoCode == "" ? null : cart.PromoCode, null, null, null, null);
        orders[order.Id] = order;
        CreateInvoice(order);
        return order;
    }

    public Invoice CreateInvoice(Order order)
    {
        var quote = order.Quote;
        // SMELL S02: invoice parts are assembled with eleven positional arguments and a null note
        var invoice = new Invoice(
            order.Id + "-I", order.Id, order.Customer, order.Lines,
            quote.Subtotal, quote.Discount, quote.AddOns, quote.Shipping, quote.Tax, quote.Total, null);
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
            new OrderConfirmedMessage(ChannelFor(order.Customer.Channel))
                .Send(order.Customer, order.Id, order.Quote.Total);
        }
        else if (order.Status == "Shipped")
        {
            new OrderShippedMessage(ChannelFor(order.Customer.Channel))
                .Send(order.Customer, order.Id, order.TrackingNumber ?? "");
        }

        loyalty.RecordStatusChange(order);
        analytics.RecordStatusChange(order);
    }

    private IChannel ChannelFor(string channel)
    {
        return channel == "email" ? emailChannel : smsChannel;
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