namespace CornerStore.Tests;

public class StrategyTests
{
    [Theory]
    [InlineData("standard", "AU", 6.60)]
    [InlineData("standard", "NZ", 9.60)]
    [InlineData("express", "AU", 15.20)]
    [InlineData("express", "NZ", 18.20)]
    [InlineData("pickup", "AU", 0.00)]
    [InlineData("pickup", "NZ", 3.00)]
    public void GivenShippingMethodAndRegion_BuildQuote_UsesExpectedShippingCost(
        string method,
        string region,
        decimal expectedShipping)
    {
        var pricing = new PricingService(new PromoEngine());

        var quote = pricing.BuildQuote(CreateCart(method), CreateCustomer(region));

        Assert.Equal(expectedShipping, quote.Shipping);
    }

    [Fact]
    public void GivenTestOnlyStrategy_BuildQuote_UsesInjectedStrategy()
    {
        var strategies = new Dictionary<string, IShippingCostStrategy>(StringComparer.Ordinal)
        {
            ["test"] = new TestShippingCostStrategy(),
        };
        var pricing = new PricingService(new PromoEngine(), strategies);

        var quote = pricing.BuildQuote(CreateCart("test"), CreateCustomer("AU"));

        Assert.Equal(17m, quote.Shipping);
    }

    private static Cart CreateCart(string method)
    {
        var cart = new Cart("cu-1")
        {
            ShippingMethod = method,
        };
        cart.Lines.Add(new CartLine
        {
            Product = new PhysicalProduct
            {
                Sku = "MUG-ENAMEL",
                Name = "Enamel Mug",
                CategoryName = "Home",
                Price = 10m,
                WeightKg = 0.8m,
            },
            Quantity = 1,
            UnitPrice = 10m,
        });
        return cart;
    }

    private static Customer CreateCustomer(string region)
    {
        return new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = region,
            Channel = "email",
            Address = "1 Harbour St",
            CreditLimit = 500m,
        };
    }

    private sealed class TestShippingCostStrategy : IShippingCostStrategy
    {
        public decimal Calculate(decimal weightKg)
        {
            return 17m;
        }
    }
}