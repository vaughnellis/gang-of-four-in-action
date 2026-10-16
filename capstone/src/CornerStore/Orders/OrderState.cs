namespace CornerStore;

internal abstract class OrderState
{
    public abstract string Name { get; }

    public virtual string? Pay(OrderService service, Order order) => Cannot("pay", order);

    public virtual string? Ship(OrderService service, Order order) => Cannot("ship", order);

    public virtual string? Deliver(OrderService service, Order order) => Cannot("deliver", order);

    public virtual string? Cancel(OrderService service, Order order) => Cannot("cancel", order);

    public virtual string? Refund(OrderService service, Order order) => Cannot("refund", order);

    protected static string Cannot(string operation, Order order)
    {
        return "cannot " + operation + " from " + order.Status;
    }
}
