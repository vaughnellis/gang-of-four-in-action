namespace CornerStore;

internal class AfterpayProcessor : IPaymentProcessor
{
    public void Charge(Customer customer, decimal amount)
    {
        Log.Info("Afterpay charged " + MoneyFormatter.Format(amount, customer.Region) + " for " + customer.Name);
    }
}