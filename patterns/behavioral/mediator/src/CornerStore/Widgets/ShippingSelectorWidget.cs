namespace CornerStore;

internal class ShippingSelectorWidget(Cart cart, PricingService pricing, CartMediator mediator)
{
    public bool CanSelect(string method)
    {
        return pricing.ShippingCost(method, 0m) is not null;
    }

    public void Select(string method)
    {
        cart.ShippingMethod = method;
        mediator.ShippingChanged();
    }

    public void Refresh()
    {
        Log.Info("shipping " + cart.ShippingMethod);
    }

}