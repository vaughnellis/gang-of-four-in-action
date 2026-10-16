namespace CornerStore;

internal sealed class OrderBuilder
{
    private string? id;
    private Customer? customer;
    private List<CartLine>? lines;
    private Quote? quote;
    private string? paymentType;
    private string? promoCode;
    private string? giftMessage;
    private string? note;
    private string? trackingNumber;
    private string? cancellationReason;

    public OrderBuilder WithId(string value)
    {
        id = value;
        return this;
    }

    public OrderBuilder WithCustomer(Customer value)
    {
        customer = value;
        return this;
    }

    public OrderBuilder WithLines(List<CartLine> value)
    {
        lines = value;
        return this;
    }

    public OrderBuilder WithQuote(Quote value)
    {
        quote = value;
        return this;
    }

    public OrderBuilder WithPaymentType(string value)
    {
        paymentType = value;
        return this;
    }

    public OrderBuilder WithPromoCode(string? value)
    {
        promoCode = value;
        return this;
    }

    public OrderBuilder WithGiftMessage(string? value)
    {
        giftMessage = value;
        return this;
    }

    public OrderBuilder WithNote(string? value)
    {
        note = value;
        return this;
    }

    public OrderBuilder WithTrackingNumber(string? value)
    {
        trackingNumber = value;
        return this;
    }

    public OrderBuilder WithCancellationReason(string? value)
    {
        cancellationReason = value;
        return this;
    }

    public Order Build()
    {
        if (id is null)
        {
            throw new InvalidOperationException("order id is required");
        }

        if (customer is null)
        {
            throw new InvalidOperationException("order customer is required");
        }

        if (lines is null)
        {
            throw new InvalidOperationException("order lines are required");
        }

        if (quote is null)
        {
            throw new InvalidOperationException("order quote is required");
        }

        if (paymentType is null)
        {
            throw new InvalidOperationException("order payment type is required");
        }

        return new Order(
            id,
            customer,
            new PendingOrderState(),
            lines,
            quote,
            paymentType,
            promoCode,
            giftMessage,
            note,
            trackingNumber,
            cancellationReason);
    }
}