namespace CornerStore;

internal class PayPalProcessor : IPaymentProcessor
{
    public void Charge(Customer customer, decimal amount)
    {
        Log.Info("PayPal charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
    }
}