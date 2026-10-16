namespace CornerStore;

internal class ShippingSelectorWidget(Cart cart, PricingService pricing)
{
    private ItemListWidget? itemList;
    private PromoBoxWidget? promoBox;
    private TotalsPanelWidget? totalsPanel;

    public void Connect(ItemListWidget itemListWidget, PromoBoxWidget promoBoxWidget, TotalsPanelWidget totalsPanelWidget)
    {
        itemList = itemListWidget;
        promoBox = promoBoxWidget;
        totalsPanel = totalsPanelWidget;
    }

    public bool CanSelect(string method)
    {
        return pricing.ShippingCost(method, 0m) is not null;
    }

    public void Select(string method)
    {
        cart.ShippingMethod = method;
    }

    public void Refresh()
    {
        Log.Instance.Info("shipping " + cart.ShippingMethod);
    }

    public void RefreshAfterChange()
    {
        // SMELL S17: this widget refreshes its peers directly after a shipping change
        Refresh();
        itemList?.Refresh();
        promoBox?.Refresh();
        totalsPanel?.Refresh();
    }
}