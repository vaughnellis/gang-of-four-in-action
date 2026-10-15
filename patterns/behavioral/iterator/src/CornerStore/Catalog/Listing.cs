namespace CornerStore;

// SMELL S11: every listing carries its own copy of the brand, manufacturer and spec text
internal class Listing
{
    public string Sku { get; init; } = "";

    public string Brand { get; init; } = "";

    public string Manufacturer { get; init; } = "";

    public string SpecText { get; init; } = "";

    public decimal Price { get; init; }
}
