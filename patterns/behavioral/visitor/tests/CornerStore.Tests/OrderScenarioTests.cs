namespace CornerStore.Tests;

public class OrderScenarioTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void AdminStockActions_AllowStaffAndRejectCustomers()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "role": "customer" }""",
            """{ "op": "register", "name": "Sam", "region": "AU", "channel": "email", "role": "staff" }""",
            """{ "op": "admin.stock.adjust", "customer": "cu-2", "sku": "KETTLE-1L", "delta": -4 }""",
            """{ "op": "admin.stock.lookup", "customer": "cu-2", "sku": "KETTLE-1L" }""",
            """{ "op": "admin.stock.adjust", "customer": "cu-1", "sku": "KETTLE-1L", "delta": 4 }""",
            """{ "op": "admin.stock.lookup", "customer": "cu-1", "sku": "KETTLE-1L" }""");

        Assert.Contains("  stock KETTLE-1L adjusted by -4, now 0\n", transcript);
        Assert.Contains("  stock KETTLE-1L 0\n", transcript);
        Assert.Equal(2, CountOccurrences(transcript, "  rejected: staff only\n"));
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "orders", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var start = 0;
        while ((start = text.IndexOf(value, start, StringComparison.Ordinal)) >= 0)
        {
            count++;
            start += value.Length;
        }

        return count;
    }
}