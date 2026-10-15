namespace CornerStore;

internal sealed class OrderShippedMessage(IChannel channel) : OrderMessage(channel)
{
    public void Send(Customer customer, string orderId, string trackingNumber)
    {
        Deliver(customer, "order " + orderId + " shipped, tracking " + trackingNumber);
    }
}
