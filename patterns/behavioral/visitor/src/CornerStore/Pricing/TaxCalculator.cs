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
        var visitor = new TaxVisitor(amount, rate);
        product.Accept(visitor);
        return visitor.Tax;
    }
}
