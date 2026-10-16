namespace CornerStore;

internal class Listing(ProductSpec productSpec)
{
    public string Sku { get; init; } = "";

    public ProductSpec Spec { get; } = productSpec;

    public decimal Price { get; init; }
}
