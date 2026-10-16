namespace CornerStore;

internal sealed class AnalyticsOrderStatusObserver(AnalyticsService analytics) : IOrderStatusObserver
{
    public void OnStatusChanged(Order order)
    {
        analytics.RecordStatusChange(order);
    }
}
