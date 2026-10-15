namespace CornerStore;

internal class Invoice(
    string id,
    string orderId,
    Customer customer,
    List<CartLine> lines,
    decimal subtotal,
    decimal discount,
    decimal addOns,
    decimal shipping,
    decimal tax,
    decimal total,
    string? note)
{
    public string Id { get; } = id;

    public string OrderId { get; } = orderId;

    public Customer Customer { get; } = customer;

    public List<CartLine> Lines { get; } = lines;

    public decimal Subtotal { get; } = subtotal;

    public decimal Discount { get; } = discount;

    public decimal AddOns { get; } = addOns;

    public decimal Shipping { get; } = shipping;

    public decimal Tax { get; } = tax;

    public decimal Total { get; } = total;

    public string? Note { get; } = note;
}