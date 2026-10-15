namespace CornerStore;

internal class CartLine
{
    public required Product Product;

    public int Quantity;

    public decimal UnitPrice;

    public string FromBundle = "";
}
