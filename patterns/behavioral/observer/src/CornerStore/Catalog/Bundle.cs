namespace CornerStore;

internal class Bundle(string sku, string name)
{
    public string Sku { get; } = sku;

    public string Name { get; } = name;

    public int DiscountPercent { get; init; } = 10;

    public List<string> ComponentSkus { get; } = [];
}
