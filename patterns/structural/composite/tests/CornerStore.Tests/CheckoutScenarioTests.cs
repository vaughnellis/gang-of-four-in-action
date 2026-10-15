namespace CornerStore.Tests;

public class CheckoutScenarioTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Checkout_ChargesBooksShipmentAndConfirmsOrder()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 500, "address": "1 Harbour St" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
            """{ "op": "checkout", "customer": "cu-1", "payment": "card" }""");

        Assert.Contains("  card charged A$", transcript);
        Assert.Contains("  ShipFast booked SF-0001 for o-1001", transcript);
        Assert.Contains("  email to Ava: order o-1001 confirmed", transcript);
        Assert.EndsWith("  checkout placed o-1001\n", transcript);
    }

    [Fact]
    public void Checkout_WhenAddressIsMissing_DoesNotChargePayment()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ben", "region": "NZ", "channel": "sms", "creditLimit": 500 }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
            """{ "op": "checkout", "customer": "cu-1", "payment": "paypal" }""");

        Assert.Contains("  checkout rejected: address required\n", transcript);
        Assert.DoesNotContain("PayPal charged", transcript);
    }

    [Fact]
    public void Checkout_WhenPlainAndBundleLinesExceedStock_DoesNotChargePayment()
    {
        var transcript = RunSteps(
            """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 5000, "address": "1 Harbour St" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42", "qty": 8 }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-RUN-KIT" }""",
            """{ "op": "checkout", "customer": "cu-1", "payment": "card" }""");

        Assert.Contains("  checkout rejected: out of stock SHOE-RUN-42\n", transcript);
        Assert.DoesNotContain("card charged", transcript);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "checkout", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}