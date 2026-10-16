namespace CornerStore;

internal class LoyaltyService
{
    public void RecordStatusChange(Order order)
    {
        Log.Instance.Info("loyalty updated for " + order.Customer.Name + " (" + order.Status + ")");
    }
}