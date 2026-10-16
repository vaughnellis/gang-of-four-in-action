namespace CornerStore;

internal class OrderService(
    IdSequence ids,
    IStockRestorer stockRestorer)
{
    private readonly Dictionary<string, Order> orders = new(StringComparer.Ordinal);
    private readonly List<Invoice> invoices = [];
    private readonly List<IOrderStatusObserver> observers = [];

    public void Subscribe(IOrderStatusObserver observer)
    {
        observers.Add(observer);
    }

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

    internal void NotifyObservers(Order order)
    {
        foreach (var observer in observers)
        {
            observer.OnStatusChanged(order);
        }
    }

    internal void RestoreStock(Order order)
    {
        foreach (var line in order.Lines)
        {
            stockRestorer.RestoreStock(line.Product.Sku, line.Quantity);
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