namespace CornerStore;

internal sealed class NotificationOrderStatusObserver(
    IChannel emailChannel,
    IChannel smsChannel) : IOrderStatusObserver
{
    public void OnStatusChanged(Order order)
    {
        if (order.Status == "Paid")
        {
            new OrderConfirmedMessage(ChannelFor(order.Customer.Channel))
                .Send(order.Customer, order.Id, order.Quote.Total);
        }
        else if (order.Status == "Shipped")
        {
            new OrderShippedMessage(ChannelFor(order.Customer.Channel))
                .Send(order.Customer, order.Id, order.TrackingNumber ?? "");
        }
    }

    private IChannel ChannelFor(string channel)
    {
        return channel == "email" ? emailChannel : smsChannel;
    }
}
