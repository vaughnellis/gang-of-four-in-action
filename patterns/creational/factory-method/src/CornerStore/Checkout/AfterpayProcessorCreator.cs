namespace CornerStore;

internal sealed class AfterpayProcessorCreator : PaymentProcessorCreator
{
    protected override IPaymentProcessor CreateProcessor()
    {
        return new AfterpayProcessor();
    }
}