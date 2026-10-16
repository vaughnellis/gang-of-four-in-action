namespace CornerStore;

internal class Cart(string customerId)
{
    public string CustomerId = customerId;

    public List<CartLine> Lines = [];

    public string PromoCode = "";

    public string ShippingMethod = "standard";

    public bool GiftWrap;

    public bool Insurance;

    public CartMemento CreateMemento()
    {
        return new CartMemento(
            CopyLines(Lines),
            PromoCode,
            ShippingMethod,
            GiftWrap,
            Insurance);
    }

    public void Restore(CartMemento memento)
    {
        memento.Restore(this);
    }

    internal void RestoreState(
        List<CartLine> lines,
        string promoCode,
        string shippingMethod,
        bool giftWrap,
        bool insurance)
    {
        Lines = CopyLines(lines);
        PromoCode = promoCode;
        ShippingMethod = shippingMethod;
        GiftWrap = giftWrap;
        Insurance = insurance;
    }

    private static List<CartLine> CopyLines(List<CartLine> lines)
    {
        var copy = new List<CartLine>();
        foreach (var line in lines)
        {
            copy.Add(new CartLine
            {
                Product = line.Product,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                FromBundle = line.FromBundle,
            });
        }

        return copy;
    }
}
