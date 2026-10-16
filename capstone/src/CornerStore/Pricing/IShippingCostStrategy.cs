namespace CornerStore;

internal interface IShippingCostStrategy
{
    decimal Calculate(decimal weightKg);
}