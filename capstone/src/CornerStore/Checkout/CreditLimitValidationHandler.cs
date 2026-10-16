namespace CornerStore;

internal sealed class CreditLimitValidationHandler(OrderValidationHandler? next) : OrderValidationHandler(next)
{
    protected override string? Validate(OrderValidationContext context)
    {
        return context.Quote.Total > context.Customer.CreditLimit
            ? "total exceeds credit limit"
            : null;
    }
}