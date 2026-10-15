namespace CornerStore.Tests;

public class ReportingScenarioTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Reports_ExportTotalsForEveryLineItemType()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 1000, "address": "1 Harbour St" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "EBOOK-GARDEN" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "GC-25" }""",
            """{ "op": "checkout", "customer": "cu-1", "payment": "card" }""",
            """{ "op": "report.csv", "status": "all" }""",
            """{ "op": "report.json", "status": "all" }""");

        Assert.Contains("  Physical,14.50\n", transcript);
        Assert.Contains("  Digital,7.50\n", transcript);
        Assert.Contains("  GiftCard,25.00\n", transcript);
        Assert.Contains("  { \"type\": \"Physical\", \"total\": 14.50 },\n", transcript);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "reports", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}