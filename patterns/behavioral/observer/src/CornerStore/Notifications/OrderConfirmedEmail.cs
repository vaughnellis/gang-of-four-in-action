namespace CornerStore;

internal class OrderConfirmedEmail(EmailSender sender)
{
    // SMELL S07: confirmation email owns the email-specific wording and sender call
    public void Send(Customer customer, string orderId, decimal total)
    {
        sender.Send(customer.Name, "order " + orderId + " confirmed, total " + MoneyFormatter.Format(total, customer.Region));
    }
}