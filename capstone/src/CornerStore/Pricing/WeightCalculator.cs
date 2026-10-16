namespace CornerStore;

internal static class WeightCalculator
{
    public static decimal WeightOf(Product product, int quantity)
    {
        var visitor = new WeightVisitor(quantity);
        product.Accept(visitor);
        return visitor.Weight;
    }
}
