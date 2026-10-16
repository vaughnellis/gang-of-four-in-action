namespace CornerStore;

internal sealed class PaidOrderState : OrderState
{
    public override string Name => "Paid";

    public override string? Ship(OrderService service, Order order)
    {
        order.State = new ShippedOrderState();
        service.AssignTrackingNumber(order);
        service.NotifyObservers(order);
        return null;
    }

    public override string? Cancel(OrderService service, Order order)
    {
        order.State = new CancelledOrderState();
        RestoreStockIfReserved(service, order);
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
