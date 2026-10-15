namespace CornerStore;

internal static class TaxCalculator
{
    public static decimal RateFor(string region)
    {
        // SMELL S01: the tax rate is picked with its own region branch
        if (region == "AU")
        {
            return 0.10m;
        }
        else if (region == "NZ")
        {
            return 0.15m;
        }

        return 0m;
    }

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
