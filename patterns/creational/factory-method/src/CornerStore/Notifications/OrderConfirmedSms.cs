namespace CornerStore;

internal class OrderConfirmedSms(SmsSender sender)
{
    // SMELL S07: confirmation SMS owns the SMS-specific wording and sender call
    public void Send(Customer customer, string orderId, decimal total)
    {
        sender.Send(customer.Name, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
    }
}