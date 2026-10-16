namespace CornerStore;

internal sealed class CartMemento
{
    private readonly List<CartLine> lines;
    private readonly string promoCode;
    private readonly string shippingMethod;
    private readonly bool giftWrap;
    private readonly bool insurance;

    internal CartMemento(
        List<CartLine> lines,
        string promoCode,
        string shippingMethod,
        bool giftWrap,
        bool insurance)
    {
        this.lines = lines;
        this.promoCode = promoCode;
        this.shippingMethod = shippingMethod;
        this.giftWrap = giftWrap;
        this.insurance = insurance;
    }

    internal void Restore(Cart cart)
    {
        cart.RestoreState(lines, promoCode, shippingMethod, giftWrap, insurance);
    }
}
