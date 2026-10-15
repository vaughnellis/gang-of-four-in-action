namespace CornerStore;

internal class PromoBoxWidget(Cart cart, Customer customer, CartService cartService, PromoEngine promos, CartMediator mediator)
{
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
        mediator.PromoChanged();
    }

    public void Refresh()
    {
        Log.Info("promo " + (cart.PromoCode == "" ? "none" : cart.PromoCode));
    }

}