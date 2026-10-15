namespace CornerStore.Tests;

public class PrototypeTests
{
    [Fact]
    public void CreateVariant_ClonesProductAndAppliesRequestedVariantDetails()
    {
        var catalog = new Catalog();
        var parent = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-42"));

        var error = catalog.CreateVariant("SHOE-RUN-42", "SHOE-RUN-44", "44", "Blue");

        var variant = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-44"));

        Assert.Null(error);
        Assert.NotSame(parent, variant);
        Assert.Equal("SHOE-RUN-44", variant.Sku);
        Assert.Equal("Runner Pro (44, Blue)", variant.Name);
        Assert.Equal("44", variant.Size);
        Assert.Equal("Blue", variant.Colour);
    }

    [Fact]
    public void CreateVariant_PreservesPrototypeAttributesAndLeavesItUnchanged()
    {
        var catalog = new Catalog();
        var parent = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-42"));
        var original = new
        {
            parent.Sku,
            parent.Name,
            parent.Price,
            parent.CategoryName,
            parent.Brand,
            parent.Manufacturer,
            parent.WeightKg,
            parent.Size,
            parent.Colour,
            parent.Material,
            parent.Description,
        };

        Assert.Null(catalog.CreateVariant("SHOE-RUN-42", "SHOE-RUN-44", "44", "Blue"));

        var variant = Assert.IsType<PhysicalProduct>(catalog.Lookup("SHOE-RUN-44"));

        Assert.Equal(original.Price, variant.Price);
        Assert.Equal(original.CategoryName, variant.CategoryName);
        Assert.Equal(original.Brand, variant.Brand);
        Assert.Equal(original.Manufacturer, variant.Manufacturer);
        Assert.Equal(original.WeightKg, variant.WeightKg);
        Assert.Equal(original.Material, variant.Material);
        Assert.Equal(original.Description, variant.Description);
        Assert.Equal(original, new
        {
            parent.Sku,
            parent.Name,
            parent.Price,
            parent.CategoryName,
            parent.Brand,
            parent.Manufacturer,
            parent.WeightKg,
            parent.Size,
            parent.Colour,
            parent.Material,
            parent.Description,
        });
    }
}
