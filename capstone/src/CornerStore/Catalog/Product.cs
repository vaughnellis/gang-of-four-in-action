namespace CornerStore;

internal abstract class Product : ICatalogItem
{
    public string Sku { get; init; } = "";

    public string Name { get; init; } = "";

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = "";

    public IReadOnlyList<ICatalogItem> Components => Array.Empty<ICatalogItem>();

    internal abstract void Accept(ILineItemVisitor visitor);

    public string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice)
    {
        CartService.AddLine(cart, this, quantity, unitPrice ?? Price, fromBundle);
        return null;
    }
}
