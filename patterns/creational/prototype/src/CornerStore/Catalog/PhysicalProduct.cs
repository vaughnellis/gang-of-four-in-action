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

    public PhysicalProduct Clone(string sku, string size, string colour)
    {
        return new PhysicalProduct
        {
            Sku = sku,
            Name = Name + " (" + size + ", " + colour + ")",
            Price = Price,
            CategoryName = CategoryName,
            Brand = Brand,
            Manufacturer = Manufacturer,
            WeightKg = WeightKg,
            Size = size,
            Colour = colour,
            Material = Material,
            Description = Description,
        };
    }
}
