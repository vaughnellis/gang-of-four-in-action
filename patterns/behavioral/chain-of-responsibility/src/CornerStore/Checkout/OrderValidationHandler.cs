namespace CornerStore;

internal abstract class OrderValidationHandler(OrderValidationHandler? next)
{
    public string? Handle(OrderValidationContext context)
    {
        return Validate(context) ?? next?.Handle(context);
    }

    protected abstract string? Validate(OrderValidationContext context);
}