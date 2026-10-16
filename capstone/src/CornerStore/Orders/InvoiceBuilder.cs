namespace CornerStore;

internal sealed class InvoiceBuilder
{
    private Order? order;
    private string? note;

    public InvoiceBuilder ForOrder(Order value)
    {
        order = value;
        return this;
    }

    public InvoiceBuilder WithNote(string? value)
    {
        note = value;
        return this;
    }

    public Invoice Build()
    {
        if (order is null)
        {
            throw new InvalidOperationException("invoice order is required");
        }

        var quote = order.Quote;
        return new Invoice(
            order.Id + "-I",
            order.Id,
            order.Customer,
            order.Lines,
            quote.Subtotal,
            quote.Discount,
            quote.AddOns,
            quote.Shipping,
            quote.Tax,
            quote.Total,
            note);
    }
}