namespace CornerStore;

internal sealed class PayPalProcessorCreator : PaymentProcessorCreator
{
    protected override IPaymentProcessor CreateProcessor()
    {
        return new PayPalProcessor();
    }
}