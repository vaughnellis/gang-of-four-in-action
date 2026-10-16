namespace CornerStore;

internal sealed class TaxVisitor(decimal amount, decimal rate) : ILineItemVisitor
{
    public decimal Tax { get; private set; }

    public void Visit(PhysicalProduct product)
    {
        Tax = Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
    }

    public void Visit(DigitalProduct product)
    {
        Tax = Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
    }

    public void Visit(GiftCardProduct product)
    {
        Tax = 0m;
    }
}
