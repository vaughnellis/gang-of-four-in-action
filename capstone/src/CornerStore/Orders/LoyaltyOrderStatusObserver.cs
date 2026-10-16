namespace CornerStore;

internal sealed class LoyaltyOrderStatusObserver(LoyaltyService loyalty) : IOrderStatusObserver
{
    public void OnStatusChanged(Order order)
    {
        loyalty.RecordStatusChange(order);
    }
}
