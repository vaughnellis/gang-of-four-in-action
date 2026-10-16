namespace CornerStore.Tests;

public class CompositeTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void GivenNestedBundle_WhenAddedToBundle_PreservesTheNestingRule()
    {
        var transcript = RunSteps(
            """{ "op": "bundle.create", "sku": "BUN-OUTER", "name": "Outer kit" }""",
            """{ "op": "bundle.add", "bundle": "BUN-OUTER", "sku": "BUN-RUN-KIT" }""");

        Assert.Contains("  rejected: BUN-RUN-KIT is a bundle and bundles cannot nest\n", transcript);
    }

    [Fact]
    public void GivenEmptyBundle_WhenAddedToCart_ReportsNoComponents()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""",
            """{ "op": "bundle.create", "sku": "BUN-EMPTY", "name": "Empty kit" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-EMPTY" }""");

        Assert.Contains("  rejected: BUN-EMPTY has no components\n", transcript);
    }

    [Fact]
    public void GivenSimpleProductAndOneLevelBundle_WhenAddedToCart_PreservesPrices()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-RUN-KIT" }""",
            """{ "op": "cart.show", "customer": "cu-1" }""");

        Assert.Contains("  - MUG-ENAMEL Enamel Mug x1 @ A$14.50 = A$14.50\n", transcript);
        Assert.Contains("  - SHOE-RUN-42 Runner Pro x1 @ A$80.96 = A$80.96 (from BUN-RUN-KIT)\n", transcript);
        Assert.Contains("  - SOCK-WOOL Wool Socks x1 @ A$10.80 = A$10.80 (from BUN-RUN-KIT)\n", transcript);
        Assert.EndsWith("  cart cu-1 lines=3 subtotal=A$106.26\n", transcript);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "composite", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}
