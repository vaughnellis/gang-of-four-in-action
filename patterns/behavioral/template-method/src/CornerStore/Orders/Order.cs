namespace CornerStore;

internal class Order(
    string id,
    Customer customer,
    string status,
    List<CartLine> lines,
    Quote quote,
    string paymentType,
    string? promoCode,
    string? giftMessage,
    string? note,
    string? trackingNumber,
    string? cancellationReason)
{
    public string Id { get; } = id;

    public Customer Customer { get; } = customer;

    public string Status { get; set; } = status;

    public List<CartLine> Lines { get; } = lines;

    public Quote Quote { get; } = quote;

    public string PaymentType { get; } = paymentType;

    public string? PromoCode { get; } = promoCode;

    public string? GiftMessage { get; } = giftMessage;

    public string? Note { get; } = note;

    public string? TrackingNumber { get; set; } = trackingNumber;

    public string? CancellationReason { get; } = cancellationReason;

    public bool StockReserved { get; set; }
}