namespace CornerStore;

internal sealed class PendingOrderState : OrderState
{
    public override string Name => "Pending";

    public override string? Pay(OrderService service, Order order)
    {
        order.State = new PaidOrderState();
        service.RecordStatusChange(order);
        return null;
    }

    public override string? Cancel(OrderService service, Order order)
    {
        order.State = new CancelledOrderState();
        RestoreStockIfReserved(service, order);
        service.RecordStatusChange(order);
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
