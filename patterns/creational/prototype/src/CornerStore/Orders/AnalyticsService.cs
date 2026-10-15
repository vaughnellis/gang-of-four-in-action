namespace CornerStore;

internal class AnalyticsService
{
    public void RecordStatusChange(Order order)
    {
        Log.Info("analytics order " + order.Id + " " + order.Status);
    }
}