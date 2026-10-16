namespace CornerStore;

internal static class TaxCalculator
{
    public static decimal LineTax(Product product, decimal amount, decimal rate)
    {
        var visitor = new TaxVisitor(amount, rate);
        product.Accept(visitor);
        return visitor.Tax;
    }
}
