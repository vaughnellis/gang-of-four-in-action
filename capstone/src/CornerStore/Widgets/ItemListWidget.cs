using System.Globalization;

namespace CornerStore;

internal class ItemListWidget(Cart cart, Customer customer, CartService cartService, CartMediator mediator)
{
    public string? Add(string sku, int quantity)
    {
        var error = cartService.Add(cart, sku, quantity);
        if (error is null)
        {
            mediator.ItemListChanged();
        }

        return error;
    }

    public string? Remove(string sku)
    {
        var error = cartService.Remove(cart, sku);
        if (error is null)
        {
            mediator.ItemListChanged();
        }

        return error;
    }

    public void Refresh()
    {
        foreach (var line in cart.Lines)
        {
            var origin = line.FromBundle == "" ? "" : " (from " + line.FromBundle + ")";
            var amount = line.UnitPrice * line.Quantity;
            Log.Instance.Info("- " + line.Product.Sku + " " + line.Product.Name + " x" + line.Quantity.ToString(CultureInfo.InvariantCulture)
                + " @ " + MoneyFormatter.Format(line.UnitPrice, customer.Region) + " = "
                + MoneyFormatter.Format(amount, customer.Region) + origin);
        }
    }

}