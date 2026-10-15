namespace CornerStore;

internal abstract class OrderMessage(IChannel channel)
{
    protected void Deliver(Customer customer, string message)
    {
        channel.Send(customer.Name, message);
    }
}
