namespace CornerStore;

internal sealed class OrderValidator
{
    private readonly OrderValidationHandler firstHandler;

    public OrderValidator(IInventoryService inventory, FraudScorer fraudScorer)
    {
        firstHandler = new StockValidationHandler(
            inventory,
            new AddressValidationHandler(
                new FraudValidationHandler(
                    fraudScorer,
                    new CreditLimitValidationHandler(null))));
    }

    public string? Validate(Customer customer, Cart cart, Quote quote)
    {
        return firstHandler.Handle(new OrderValidationContext(customer, cart, quote));
    }
}