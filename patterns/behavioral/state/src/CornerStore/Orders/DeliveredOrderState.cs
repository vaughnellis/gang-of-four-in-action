namespace CornerStore;

internal sealed class DeliveredOrderState : OrderState
{
    public override string Name => "Delivered";

    public override string? Refund(OrderService service, Order order)
    {
        order.State = new RefundedOrderState();
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