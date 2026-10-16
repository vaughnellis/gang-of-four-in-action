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
        if (item is null)
        {
            return "unknown sku " + sku;
        }

        return item.AddToCart(cart, quantity, "", null);
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

    internal static void AddLine(Cart cart, Product product, int quantity, decimal unitPrice, string fromBundle)
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
