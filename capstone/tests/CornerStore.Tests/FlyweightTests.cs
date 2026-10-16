namespace CornerStore.Tests;

public class FlyweightTests
{
    [Fact]
    public void Import_ReusesTheSameProductSpecForRepeatedIntrinsicData()
    {
        var importer = new ListingImporter();

        var listings = importer.Import(["SHOE-RUN-42"], 120);

        Assert.Same(listings[0].Spec, listings[60].Spec);
        Assert.Equal("Stridewell & Co", listings[0].Spec.Brand);
        Assert.Equal("Stridewell Manufacturing Pty Ltd", listings[0].Spec.Manufacturer);
        Assert.Equal("Material: Mesh; origin: Portugal; care: machine wash cold", listings[0].Spec.SpecText);
    }

    [Fact]
    public void Import_KeepsSkuAndPriceSpecificToEachListing()
    {
        var importer = new ListingImporter();

        var listings = importer.Import(["SHOE-RUN-42"], 2);

        Assert.NotSame(listings[0].Spec, listings[1].Spec);
        Assert.Equal("SHOE-RUN-42-L0", listings[0].Sku);
        Assert.Equal("SHOE-RUN-42-L1", listings[1].Sku);
        Assert.Equal(10m, listings[0].Price);
        Assert.Equal(11m, listings[1].Price);
    }
}
