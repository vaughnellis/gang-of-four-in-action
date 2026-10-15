namespace CornerStore;

internal class PhysicalProduct : Product
{
    public string Brand { get; init; } = "";

    public string Manufacturer { get; init; } = "";

    public decimal WeightKg { get; init; }

    public string Size { get; init; } = "";

    public string Colour { get; init; } = "";

    public string Material { get; init; } = "";

    public string Description { get; init; } = "";
}
