namespace CornerStore;

internal sealed class WeightVisitor(int quantity) : ILineItemVisitor
{
    public decimal Weight { get; private set; }

    public void Visit(PhysicalProduct product)
    {
        Weight = product.WeightKg * quantity;
    }

    public void Visit(DigitalProduct product)
    {
        Weight = 0m;
    }

    public void Visit(GiftCardProduct product)
    {
        Weight = 0m;
    }
}
