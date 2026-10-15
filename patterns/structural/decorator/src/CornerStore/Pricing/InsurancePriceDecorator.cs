namespace CornerStore;

internal sealed class InsurancePriceDecorator(
    IAddOnPriceComponent component,
    bool enabled,
    decimal discountedSubtotal) : IAddOnPriceComponent
{
    public decimal Cost => component.Cost + (enabled
        ? Math.Round(discountedSubtotal * 0.02m, 2, MidpointRounding.AwayFromZero)
        : 0m);
}