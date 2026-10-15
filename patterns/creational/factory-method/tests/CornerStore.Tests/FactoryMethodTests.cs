namespace CornerStore.Tests;

public class FactoryMethodTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void GivenCardCreator_Charge_UsesCardProcessor()
    {
        var output = Charge(new CardProcessorCreator());

        Assert.Contains("card charged", output);
    }

    [Fact]
    public void GivenPayPalCreator_Charge_UsesPayPalProcessor()
    {
        var output = Charge(new PayPalProcessorCreator());

        Assert.Contains("PayPal charged", output);
    }

    [Fact]
    public void GivenAfterpayCreator_Charge_UsesAfterpayProcessor()
    {
        var output = Charge(new AfterpayProcessorCreator());

        Assert.Contains("Afterpay charged", output);
    }

    [Fact]
    public void GivenUnknownPaymentType_Checkout_RejectsWithoutCharging()
    {
        File.WriteAllText(
            _path,
            """{ "name": "factory-method", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": ["""
            + """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 500, "address": "1 Harbour St" },"""
            + """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" },"""
            + """{ "op": "checkout", "customer": "cu-1", "payment": "giftcard" }]}""");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        Assert.Contains("checkout rejected: unknown payment type giftcard", output.ToString());
        Assert.DoesNotContain("charged", output.ToString());
    }

    private static string Charge(PaymentProcessorCreator creator)
    {
        var previousOutput = Log.Out;
        var output = new StringWriter();
        Log.Out = output;

        try
        {
            creator.Charge(
                new Customer { Name = "Ava", Region = "AU" },
                10m);
            return output.ToString();
        }
        finally
        {
            Log.Out = previousOutput;
        }
    }
}