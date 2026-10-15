using System.Globalization;

namespace CornerStore;

internal class ItemListWidget(Cart cart, Customer customer, CartService cartService)
{
    private PromoBoxWidget? promoBox;
    private ShippingSelectorWidget? shippingSelector;
    private TotalsPanelWidget? totalsPanel;

    public PromoBoxWidget PromoBox => promoBox ?? throw new InvalidOperationException("widgets are not connected");

    public ShippingSelectorWidget ShippingSelector => shippingSelector ?? throw new InvalidOperationException("widgets are not connected");

    public TotalsPanelWidget TotalsPanel => totalsPanel ?? throw new InvalidOperationException("widgets are not connected");

    public string? Add(string sku, int quantity)
    {
        return cartService.Add(cart, sku, quantity);
    }

    public string? Remove(string sku)
    {
        return cartService.Remove(cart, sku);
    }

    public void Connect(PromoBoxWidget promoBoxWidget, ShippingSelectorWidget shippingSelectorWidget, TotalsPanelWidget totalsPanelWidget)
    {
        promoBox = promoBoxWidget;
        shippingSelector = shippingSelectorWidget;
        totalsPanel = totalsPanelWidget;
    }

    public void Refresh()
    {
        foreach (var line in cart.Lines)
        {
            var origin = line.FromBundle == "" ? "" : " (from " + line.FromBundle + ")";
            var amount = line.UnitPrice * line.Quantity;
            Log.Info("- " + line.Product.Sku + " " + line.Product.Name + " x" + line.Quantity.ToString(CultureInfo.InvariantCulture)
                + " @ " + MoneyFormatter.Format(line.UnitPrice, customer.Region) + " = "
                + MoneyFormatter.Format(amount, customer.Region) + origin);
        }
    }

    public void RefreshAfterChange()
    {
        // SMELL S17: this widget refreshes its peers directly after a cart change
        Refresh();
        promoBox?.Refresh();
        shippingSelector?.Refresh();
        totalsPanel?.Refresh();
    }
}