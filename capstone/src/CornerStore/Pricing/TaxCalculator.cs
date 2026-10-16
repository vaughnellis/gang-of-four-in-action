namespace CornerStore;

internal static class TaxCalculator
{
    public static decimal LineTax(Product product, decimal amount, decimal rate)
    {
        // SMELL S23: tax depends on the line type, picked with a type switch
        switch (product)
        {
            case PhysicalProduct:
                return Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
            case DigitalProduct:
                return Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
            case GiftCardProduct:
                return 0m;
            default:
                throw new InvalidOperationException("unknown product type for " + product.Sku);
        }
    }
}
