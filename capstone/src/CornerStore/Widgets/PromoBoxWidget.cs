namespace CornerStore;

internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartService, PromoEngine promos)
{
    private ItemListWidget? itemList;
    private ShippingSelectorWidget? shippingSelector;
    private TotalsPanelWidget? totalsPanel;

    public void Connect(ItemListWidget itemListWidget, ShippingSelectorWidget shippingSelectorWidget, TotalsPanelWidget totalsPanelWidget)
    {
        itemList = itemListWidget;
        shippingSelector = shippingSelectorWidget;
        totalsPanel = totalsPanelWidget;
    }

    public bool IsKnown(string code)
    {
        return promos.IsKnown(code);
    }

    public bool Evaluate(string code)
    {
        return promos.Evaluate(code, cart, customer, cartService.Subtotal(cart));
    }

    public void Apply(string code)
    {
        cart.PromoCode = code;
    }

    public void Refresh()
    {
        Log.Instance.Info("promo " + (cart.PromoCode == "" ? "none" : cart.PromoCode));
    }

    public void RefreshAfterChange()
    {
        // SMELL S17: this widget refreshes its peers directly after a promo change
        Refresh();
        itemList?.Refresh();
        shippingSelector?.Refresh();
        totalsPanel?.Refresh();
    }
}