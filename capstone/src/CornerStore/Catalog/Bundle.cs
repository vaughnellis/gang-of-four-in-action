namespace CornerStore;

internal class Bundle(string sku, string name) : ICatalogItem
{
    private readonly List<ICatalogItem> components = [];

    public string Sku { get; } = sku;

    public string Name { get; } = name;

    public int DiscountPercent { get; init; } = 10;

    public IReadOnlyList<ICatalogItem> Components => components.AsReadOnly();

    public decimal Price
    {
        get
        {
            var total = 0m;
            foreach (var component in components)
            {
                total += component.Price;
            }

            return Math.Round(total * (100 - DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
        }
    }

    public void AddComponent(ICatalogItem component)
    {
        components.Add(component);
    }

    public string? AddToCart(Cart cart, int quantity, string fromBundle, decimal? unitPrice)
    {
        if (components.Count == 0)
        {
            return Sku + " has no components";
        }

        var rootBundleSku = fromBundle == "" ? Sku : fromBundle;
        var targetPrice = unitPrice ?? Price;
        var scale = Price == 0m ? 0m : targetPrice / Price;
        var allocated = 0m;
        for (var i = 0; i < components.Count; i++)
        {
            var component = components[i];
            var discountedPrice = Math.Round(
                component.Price * (100 - DiscountPercent) / 100m,
                2,
                MidpointRounding.AwayFromZero);
            var componentPrice = i == components.Count - 1
                ? targetPrice - allocated
                : Math.Round(discountedPrice * scale, 2, MidpointRounding.AwayFromZero);
            allocated += componentPrice;

            var error = component.AddToCart(cart, quantity, rootBundleSku, componentPrice);
            if (error is not null)
            {
                return error;
            }
        }

        return null;
    }
}
