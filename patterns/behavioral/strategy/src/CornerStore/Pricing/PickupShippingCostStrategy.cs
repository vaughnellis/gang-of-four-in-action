namespace CornerStore;

internal sealed class PickupShippingCostStrategy : IShippingCostStrategy
{
    public decimal Calculate(decimal weightKg)
    {
        return 0m;
    }
}