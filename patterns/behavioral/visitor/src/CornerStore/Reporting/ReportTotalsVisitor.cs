namespace CornerStore;

internal sealed class ReportTotalsVisitor : ILineItemVisitor
{
    private decimal currentAmount;

    public decimal PhysicalTotal { get; private set; }

    public decimal DigitalTotal { get; private set; }

    public decimal GiftCardTotal { get; private set; }

    public void Add(Product product, decimal amount)
    {
        currentAmount = amount;
        product.Accept(this);
    }

    public void Visit(PhysicalProduct product)
    {
        PhysicalTotal += currentAmount;
    }

    public void Visit(DigitalProduct product)
    {
        DigitalTotal += currentAmount;
    }

    public void Visit(GiftCardProduct product)
    {
        GiftCardTotal += currentAmount;
    }
}
