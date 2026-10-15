namespace CornerStore.Tests;

public class ChainOfResponsibilityTests
{
    [Fact]
    public void GivenDuplicateSkuLinesAndMissingAddress_Validate_ReturnsSortedAggregatedStockFailureFirst()
    {
        var validator = new OrderValidator(new InventoryService(), new FraudScorer());
        var customer = CreateCustomer(address: "");
        var cart = new Cart(customer.Id);
        cart.Lines.Add(CreateLine("MUG-ENAMEL", 9));
        cart.Lines.Add(CreateLine("KETTLE-1L", 2));
        cart.Lines.Add(CreateLine("KETTLE-1L", 3));

        var error = validator.Validate(customer, cart, new Quote { Total = 1m });

        Assert.Equal("out of stock KETTLE-1L", error);
    }

    [Fact]
    public void GivenMissingAddress_Validate_ReturnsAddressFailure()
    {
        var oldThreshold = Config.FraudThreshold;
        Config.FraudThreshold = 100;
        try
        {
            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
            var customer = CreateCustomer(address: "");

            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 1m });

            Assert.Equal("address required", error);
        }
        finally
        {
            Config.FraudThreshold = oldThreshold;
        }
    }

    [Fact]
    public void GivenFraudScoreAboveThreshold_Validate_ReturnsFraudFailureBeforeCredit()
    {
        var oldThreshold = Config.FraudThreshold;
        Config.FraudThreshold = -1;
        try
        {
            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
            var customer = CreateCustomer(creditLimit: 0m);

            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 1m });

            Assert.StartsWith("fraud score ", error);
            Assert.EndsWith(" exceeds threshold", error);
        }
        finally
        {
            Config.FraudThreshold = oldThreshold;
        }
    }

    [Fact]
    public void GivenTotalAboveCreditLimit_Validate_ReturnsCreditFailure()
    {
        var oldThreshold = Config.FraudThreshold;
        Config.FraudThreshold = 100;
        try
        {
            var validator = new OrderValidator(new InventoryService(), new FraudScorer());
            var customer = CreateCustomer(creditLimit: 1m);

            var error = validator.Validate(customer, new Cart(customer.Id), new Quote { Total = 2m });

            Assert.Equal("total exceeds credit limit", error);
        }
        finally
        {
            Config.FraudThreshold = oldThreshold;
        }
    }

    private static CartLine CreateLine(string sku, int quantity)
    {
        return new CartLine
        {
            Product = new PhysicalProduct { Sku = sku },
            Quantity = quantity,
            UnitPrice = 10m,
        };
    }

    private static Customer CreateCustomer(string address = "1 Harbour St", decimal creditLimit = 500m)
    {
        return new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = address,
            CreditLimit = creditLimit,
        };
    }
}