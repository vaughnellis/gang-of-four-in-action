namespace CornerStore;

internal class OrderShippedSms(SmsSender sender)
{
    // SMELL S07: shipping SMS owns the SMS-specific wording and sender call
    public void Send(Customer customer, string orderId, string trackingNumber)
    {
        sender.Send(customer.Name, "order " + orderId + " shipped, tracking " + trackingNumber);
    }
}