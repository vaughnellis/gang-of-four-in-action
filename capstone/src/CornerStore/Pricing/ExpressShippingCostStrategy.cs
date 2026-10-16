namespace CornerStore;

internal sealed class ExpressShippingCostStrategy : IShippingCostStrategy
{
    public decimal Calculate(decimal weightKg)
    {
        return Math.Round(12.00m + 4.00m * weightKg, 2, MidpointRounding.AwayFromZero);
    }
}