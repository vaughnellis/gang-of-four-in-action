namespace CornerStore;

internal class OrderShippedEmail(EmailSender sender)
{
    // SMELL S07: shipping email owns the email-specific wording and sender call
    public void Send(Customer customer, string orderId, string trackingNumber)
    {
        sender.Send(customer.Name, "order " + orderId + " shipped, tracking " + trackingNumber);
    }
}