namespace CornerStore;

internal abstract class Product
{
    public string Sku { get; init; } = "";

    public string Name { get; init; } = "";

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = "";
}
