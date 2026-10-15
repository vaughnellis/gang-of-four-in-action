namespace CornerStore;

internal sealed class AddressValidationHandler(OrderValidationHandler? next) : OrderValidationHandler(next)
{
    protected override string? Validate(OrderValidationContext context)
    {
        return context.Customer.Address == "" ? "address required" : null;
    }
}