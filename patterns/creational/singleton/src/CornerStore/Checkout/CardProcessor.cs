namespace CornerStore;

internal class CardProcessor : IPaymentProcessor
{
    public void Charge(Customer customer, decimal amount)
    {
        Log.Instance.Info("card charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
    }
}