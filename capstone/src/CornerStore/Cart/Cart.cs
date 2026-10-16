namespace CornerStore;

// SMELL S18: every internal of the cart is a public field, so a caller can copy the state out and back in
internal class Cart(string customerId)
{
    public string CustomerId = customerId;

    public List<CartLine> Lines = [];

    public string PromoCode = "";

    public string ShippingMethod = "standard";

    public bool GiftWrap;

    public bool Insurance;
}
