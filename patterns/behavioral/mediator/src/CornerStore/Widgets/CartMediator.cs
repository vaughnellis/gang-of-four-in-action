namespace CornerStore;

internal sealed class CartMediator
{
    private readonly ItemListWidget itemListWidget;
    private readonly PromoBoxWidget promoBoxWidget;
    private readonly ShippingSelectorWidget shippingSelectorWidget;
    private readonly TotalsPanelWidget totalsPanelWidget;

    public CartMediator(Cart cart, Customer customer, CartService cartService, PromoEngine promos, PricingService pricing)
    {
        itemListWidget = new ItemListWidget(cart, customer, cartService, this);
        promoBoxWidget = new PromoBoxWidget(cart, customer, cartService, promos, this);
        shippingSelectorWidget = new ShippingSelectorWidget(cart, pricing, this);
        totalsPanelWidget = new TotalsPanelWidget(cart, customer, pricing);
    }

    public string? Add(string sku, int quantity)
    {
        return ItemList.Add(sku, quantity);
    }

    public string? Remove(string sku)
    {
        return ItemList.Remove(sku);
    }

    public bool IsKnownPromo(string code)
    {
        return PromoBox.IsKnown(code);
    }

    public bool PromoApplies(string code)
    {
        return PromoBox.Evaluate(code);
    }

    public void ApplyPromo(string code)
    {
        PromoBox.Apply(code);
    }

    public bool CanSelectShipping(string method)
    {
        return ShippingSelector.CanSelect(method);
    }

    public void SelectShipping(string method)
    {
        ShippingSelector.Select(method);
    }

    public void ShowQuote()
    {
        TotalsPanel.ShowQuote();
    }

    public void ItemListChanged()
    {
        ItemList.Refresh();
        PromoBox.Refresh();
        ShippingSelector.Refresh();
        TotalsPanel.Refresh();
    }

    public void PromoChanged()
    {
        PromoBox.Refresh();
        ItemList.Refresh();
        ShippingSelector.Refresh();
        TotalsPanel.Refresh();
    }

    public void ShippingChanged()
    {
        ShippingSelector.Refresh();
        ItemList.Refresh();
        PromoBox.Refresh();
        TotalsPanel.Refresh();
    }

    public void TotalsChanged()
    {
        TotalsPanel.Refresh();
        ItemList.Refresh();
        PromoBox.Refresh();
        ShippingSelector.Refresh();
    }

    private ItemListWidget ItemList => itemListWidget;

    private PromoBoxWidget PromoBox => promoBoxWidget;

    private ShippingSelectorWidget ShippingSelector => shippingSelectorWidget;

    private TotalsPanelWidget TotalsPanel => totalsPanelWidget;
}
