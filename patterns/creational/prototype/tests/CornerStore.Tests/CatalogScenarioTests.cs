namespace CornerStore.Tests;

public class CatalogScenarioTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Browse_FromTheRoot_ListsTheNestedTreeAndCountsEveryProduct()
    {
        var transcript = RunSteps("""{ "op": "browse" }""");

        Assert.Contains("  Store\n", transcript);
        Assert.Contains("      Shoes\n", transcript);
        Assert.Contains("        Kids\n", transcript);
        Assert.Contains("  12 products under Store\n", transcript);
    }

    [Fact]
    public void Variant_OfAPhysicalProduct_AppearsInItsCategory()
    {
        var transcript = RunSteps(
            """{ "op": "variant", "parent": "SHOE-RUN-42", "sku": "SHOE-RUN-44", "size": "44", "colour": "Blue" }""",
            """{ "op": "browse", "category": "Shoes" }""");

        Assert.Contains("  created variant SHOE-RUN-44 of SHOE-RUN-42\n", transcript);
        Assert.Contains("- SHOE-RUN-44 Runner Pro (44, Blue) 89.95\n", transcript);
    }

    [Fact]
    public void Variant_OfADigitalProduct_IsRejected()
    {
        var transcript = RunSteps(
            """{ "op": "variant", "parent": "EBOOK-FIT", "sku": "EBOOK-FIT-2", "size": "A5", "colour": "None" }""");

        Assert.Contains("  rejected: EBOOK-FIT is not a physical product\n", transcript);
    }

    [Fact]
    public void BundleAdd_WithAnotherBundle_IsRejectedWithoutThrowing()
    {
        var transcript = RunSteps(
            """{ "op": "bundle.create", "sku": "BUN-GIFT", "name": "Gift set" }""",
            """{ "op": "bundle.add", "bundle": "BUN-GIFT", "sku": "BUN-RUN-KIT" }""");

        Assert.Contains("  rejected: BUN-RUN-KIT is a bundle and bundles cannot nest\n", transcript);
    }

    [Fact]
    public void BundleAdd_WithProducts_PricesTheBundleAtTenPercentOff()
    {
        var transcript = RunSteps(
            """{ "op": "bundle.create", "sku": "BUN-GIFT", "name": "Gift set" }""",
            """{ "op": "bundle.add", "bundle": "BUN-GIFT", "sku": "MUG-ENAMEL" }""",
            """{ "op": "bundle.add", "bundle": "BUN-GIFT", "sku": "TOWEL-SET" }""");

        Assert.Contains("  bundle BUN-GIFT price 38.25\n", transcript);
    }

    [Fact]
    public void Import_With100000Listings_ReportsOnlyTheSummaryCount()
    {
        var transcript = RunSteps("""{ "op": "import", "count": 100000 }""");

        Assert.Equal("[001] import count=100000\n  imported 100000 listings\n", transcript[(transcript.IndexOf('\n') + 1)..]);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "catalog", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}
