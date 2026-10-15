namespace CornerStore;

internal sealed class GiftWrapPriceDecorator(IAddOnPriceComponent component, bool enabled) : IAddOnPriceComponent
{
    public decimal Cost => component.Cost + (enabled ? 4.50m : 0m);
}