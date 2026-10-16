namespace CornerStore;

internal sealed class ShippedOrderState : OrderState
{
    public override string Name => "Shipped";

    public override string? Deliver(OrderService service, Order order)
    {
        order.State = new DeliveredOrderState();
        service.NotifyObservers(order);
        return null;
    }

    public override string? Refund(OrderService service, Order order)
    {
        order.State = new RefundedOrderState();
        RestoreStockIfReserved(service, order);
        service.NotifyObservers(order);
        return null;
    }

    private static void RestoreStockIfReserved(OrderService service, Order order)
    {
        if (order.StockReserved)
        {
            service.RestoreStock(order);
        }
    }
}
