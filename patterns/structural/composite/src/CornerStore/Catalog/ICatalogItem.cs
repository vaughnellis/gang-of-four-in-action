namespace CornerStore;

internal interface ICatalogItem
{
    string Sku { get; }

    string Name { get; }

    decimal Price { get; }

    IReadOnlyList<ICatalogItem> Components { get; }

    string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice);
}
