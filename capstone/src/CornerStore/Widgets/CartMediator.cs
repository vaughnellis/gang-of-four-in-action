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
        return itemListWidget.Add(sku, quantity);
    }

    public string? Remove(string sku)
    {
        return itemListWidget.Remove(sku);
    }

    public bool IsKnownPromo(string code)
    {
        return promoBoxWidget.IsKnown(code);
    }

    public bool PromoApplies(string code)
    {
        return promoBoxWidget.Evaluate(code);
    }

    public void ApplyPromo(string code)
    {
        promoBoxWidget.Apply(code);
    }

    public bool CanSelectShipping(string method)
    {
        return shippingSelectorWidget.CanSelect(method);
    }

    public void SelectShipping(string method)
    {
        shippingSelectorWidget.Select(method);
    }

    public void ShowQuote()
    {
        totalsPanelWidget.ShowQuote();
    }

    public void ItemListChanged()
    {
        itemListWidget.Refresh();
        promoBoxWidget.Refresh();
        shippingSelectorWidget.Refresh();
        totalsPanelWidget.Refresh();
    }

    public void PromoChanged()
    {
        promoBoxWidget.Refresh();
        itemListWidget.Refresh();
        shippingSelectorWidget.Refresh();
        totalsPanelWidget.Refresh();
    }

    public void ShippingChanged()
    {
        shippingSelectorWidget.Refresh();
        itemListWidget.Refresh();
        promoBoxWidget.Refresh();
        totalsPanelWidget.Refresh();
    }

    public void TotalsChanged()
    {
        totalsPanelWidget.Refresh();
        itemListWidget.Refresh();
        promoBoxWidget.Refresh();
        shippingSelectorWidget.Refresh();
    }
}
