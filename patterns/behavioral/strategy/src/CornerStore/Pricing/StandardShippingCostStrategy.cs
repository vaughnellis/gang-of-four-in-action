namespace CornerStore;

internal sealed class StandardShippingCostStrategy : IShippingCostStrategy
{
    public decimal Calculate(decimal weightKg)
    {
        return Math.Round(5.00m + 2.00m * weightKg, 2, MidpointRounding.AwayFromZero);
    }
}