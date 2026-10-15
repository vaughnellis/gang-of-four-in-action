namespace CornerStore;

internal abstract class PaymentProcessorCreator
{
    public void Charge(Customer customer, decimal amount)
    {
        CreateProcessor().Charge(customer, amount);
    }

    protected abstract IPaymentProcessor CreateProcessor();
}