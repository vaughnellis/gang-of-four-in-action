namespace CornerStore.Tests;

public class VisitorTests
{
    [Fact]
    public void TaxCalculator_LineTax_DispatchesByProductType()
    {
        var physical = new PhysicalProduct();
        var digital = new DigitalProduct();
        var giftCard = new GiftCardProduct();

        Assert.Equal(1.24m, TaxCalculator.LineTax(physical, 12.35m, 0.10m));
        Assert.Equal(1.24m, TaxCalculator.LineTax(digital, 12.35m, 0.10m));
        Assert.Equal(0m, TaxCalculator.LineTax(giftCard, 12.35m, 0.10m));
    }

    [Fact]
    public void WeightCalculator_WeightOf_UsesQuantityOnlyForPhysicalProducts()
    {
        var physical = new PhysicalProduct { WeightKg = 1.25m };
        var digital = new DigitalProduct();
        var giftCard = new GiftCardProduct();

        Assert.Equal(3.75m, WeightCalculator.WeightOf(physical, 3));
        Assert.Equal(0m, WeightCalculator.WeightOf(digital, 3));
        Assert.Equal(0m, WeightCalculator.WeightOf(giftCard, 3));
    }

    [Fact]
    public void ReportTotalsVisitor_Add_DistributesAmountsByProductType()
    {
        var visitor = new ReportTotalsVisitor();

        visitor.Add(new PhysicalProduct(), 14.50m);
        visitor.Add(new PhysicalProduct(), 5.50m);
        visitor.Add(new DigitalProduct(), 7.50m);
        visitor.Add(new GiftCardProduct(), 25m);

        Assert.Equal(20m, visitor.PhysicalTotal);
        Assert.Equal(7.50m, visitor.DigitalTotal);
        Assert.Equal(25m, visitor.GiftCardTotal);
    }
}
