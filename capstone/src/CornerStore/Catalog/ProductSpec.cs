namespace CornerStore;

internal sealed class ProductSpec(string brand, string manufacturer, string specText)
{
    public string Brand { get; } = brand;

    public string Manufacturer { get; } = manufacturer;

    public string SpecText { get; } = specText;
}
