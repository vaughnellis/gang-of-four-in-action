namespace CornerStore;

internal sealed class NotificationOrderStatusObserver(
    OrderConfirmedEmail confirmedEmail,
    OrderConfirmedSms confirmedSms,
    OrderShippedEmail shippedEmail,
    OrderShippedSms shippedSms) : IOrderStatusObserver
{
    public void OnStatusChanged(Order order)
    {
        if (order.Status == "Paid")
        {
            if (order.Customer.Channel == "email")
            {
                confirmedEmail.Send(order.Customer, order.Id, order.Quote.Total);
            }
            else
            {
                confirmedSms.Send(order.Customer, order.Id, order.Quote.Total);
            }
        }
        else if (order.Status == "Shipped")
        {
            if (order.Customer.Channel == "email")
            {
                shippedEmail.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
            }
            else
            {
                shippedSms.Send(order.Customer, order.Id, order.TrackingNumber ?? "");
            }
        }
    }
}
