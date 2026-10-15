namespace CornerStore;

internal class Quote
{
    public decimal Subtotal { get; init; }

    public decimal Discount { get; init; }

    public decimal AddOns { get; init; }

    public decimal Shipping { get; init; }

    public decimal Tax { get; init; }

    public decimal Total { get; init; }

    public decimal WeightKg { get; init; }
}
