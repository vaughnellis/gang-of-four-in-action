namespace CornerStore.Tests;

public class AbstractFactoryTests
{
    [Fact]
    public void RegionFactory_ForAustralia_CreatesAustralianProductFamily()
    {
        var factory = RegionFactoryProvider.For("AU");

        Assert.Equal(0.10m, factory.CreatePricing().TaxRate);
        Assert.Equal(0m, factory.CreatePricing().ShippingSurcharge);
        Assert.Equal("A$12.50", factory.CreateMoneyFormatter().Format(12.50m));
    }

    [Fact]
    public void RegionFactory_ForNewZealand_CreatesNewZealandProductFamily()
    {
        var factory = RegionFactoryProvider.For("NZ");

        Assert.Equal(0.15m, factory.CreatePricing().TaxRate);
        Assert.Equal(3.00m, factory.CreatePricing().ShippingSurcharge);
        Assert.Equal("NZ$12.50", factory.CreateMoneyFormatter().Format(12.50m));
    }

    [Fact]
    public void RegionFactory_ForUnknownRegion_PreservesDefaultProductFamily()
    {
        var factory = RegionFactoryProvider.For("XX");

        Assert.Equal(0m, factory.CreatePricing().TaxRate);
        Assert.Equal(0m, factory.CreatePricing().ShippingSurcharge);
        Assert.Equal("12.50", factory.CreateMoneyFormatter().Format(12.50m));
    }
}
