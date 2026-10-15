namespace CornerStore;

internal sealed class OrderConfirmedMessage(IChannel channel) : OrderMessage(channel)
{
    public void Send(Customer customer, string orderId, decimal total)
    {
        Deliver(customer, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
    }
}
