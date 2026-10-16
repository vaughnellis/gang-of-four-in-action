namespace CornerStore;

internal sealed class CardProcessorCreator : PaymentProcessorCreator
{
    protected override IPaymentProcessor CreateProcessor()
    {
        return new CardProcessor();
    }
}