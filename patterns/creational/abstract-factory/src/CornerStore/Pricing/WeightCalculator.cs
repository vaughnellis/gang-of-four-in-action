namespace CornerStore;

internal static class WeightCalculator
{
    public static decimal WeightOf(Product product, int quantity)
    {
        // SMELL S23: weight depends on the line type, picked with a type switch
        switch (product)
        {
            case PhysicalProduct physical:
                return physical.WeightKg * quantity;
            case DigitalProduct:
                return 0m;
            case GiftCardProduct:
                return 0m;
            default:
                throw new InvalidOperationException("unknown product type for " + product.Sku);
        }
    }
}
