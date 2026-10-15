namespace CornerStore;

internal class CartService(Catalog catalog)
{
    public string? Add(Cart cart, string sku, int quantity)
    {
        if (quantity < 1)
        {
            return "quantity must be at least 1";
        }

        var item = catalog.Lookup(sku);

        // SMELL S08: a bundle is told apart with an is-check and expanded into discounted component lines
        if (item is Bundle bundle)
        {
            return AddBundle(cart, bundle, quantity);
        }

        if (item is not Product product)
        {
            return "unknown sku " + sku;
        }

        AddLine(cart, product, quantity, product.Price, "");
        return null;
    }

    public string? Remove(Cart cart, string sku)
    {
        var removed = cart.Lines.RemoveAll(line => line.FromBundle == sku || (line.FromBundle == "" && line.Product.Sku == sku));
        return removed == 0 ? sku + " is not in the cart" : null;
    }

    public decimal Subtotal(Cart cart)
    {
        var total = 0m;
        foreach (var line in cart.Lines)
        {
            total += line.UnitPrice * line.Quantity;
        }

        return total;
    }

    private string? AddBundle(Cart cart, Bundle bundle, int quantity)
    {
        if (bundle.ComponentSkus.Count == 0)
        {
            return bundle.Sku + " has no components";
        }

        // The last component takes the remainder so the lines add up to the bundle price.
        var bundlePrice = catalog.PriceOf(bundle.Sku);
        var allocated = 0m;
        for (var i = 0; i < bundle.ComponentSkus.Count; i++)
        {
            if (catalog.Lookup(bundle.ComponentSkus[i]) is not Product component)
            {
                return "unknown sku " + bundle.ComponentSkus[i];
            }

            var price = i == bundle.ComponentSkus.Count - 1
                ? bundlePrice - allocated
                : Math.Round(component.Price * (100 - bundle.DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
            allocated += price;
            AddLine(cart, component, quantity, price, bundle.Sku);
        }

        return null;
    }

    private static void AddLine(Cart cart, Product product, int quantity, decimal unitPrice, string fromBundle)
    {
        foreach (var line in cart.Lines)
        {
            if (line.Product.Sku == product.Sku && line.FromBundle == fromBundle)
            {
                line.Quantity += quantity;
                return;
            }
        }

        cart.Lines.Add(new CartLine { Product = product, Quantity = quantity, UnitPrice = unitPrice, FromBundle = fromBundle });
    }
}
