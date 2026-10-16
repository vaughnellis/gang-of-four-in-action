namespace CornerStore.Tests;

public class PricingScenarioTests : IDisposable
{
    private const string Ava = """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""";
    private const string Ben = """{ "op": "register", "name": "Ben", "region": "NZ", "channel": "sms", "creditLimit": 300 }""";
    private const string OneShoe = """{ "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42" }""";
    private const string TwoShoes = """{ "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42", "qty": 2 }""";
    private const string RunKit = """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-RUN-KIT" }""";
    private const string Quote = """{ "op": "quote", "customer": "cu-1" }""";

    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Quote_ForAnAuCart_UsesTenPercentTaxAndAustralianCurrency()
    {
        var transcript = RunSteps(Ava, OneShoe, Quote);

        Assert.EndsWith(
            "  quote cu-1 (AU, standard, 0.80 kg)\n  subtotal A$89.95\n  add-ons A$0.00\n  shipping A$6.60\n  tax A$9.00\n  total A$105.55\n",
            transcript);
    }

    [Fact]
    public void Quote_ForAnNzCart_UsesFifteenPercentTaxSurchargeAndNzCurrency()
    {
        var transcript = RunSteps(Ben, OneShoe, Quote);

        Assert.EndsWith(
            "  quote cu-1 (NZ, standard, 0.80 kg)\n  subtotal NZ$89.95\n  add-ons NZ$0.00\n  shipping NZ$9.60\n  tax NZ$13.49\n  total NZ$113.04\n",
            transcript);
    }

    [Fact]
    public void Quote_ForGiftCardAndDigitalLines_HasNoWeightAndTaxesOnlyTheDigitalLine()
    {
        var transcript = RunSteps(
            Ava,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "GC-25" }""",
            """{ "op": "cart.add", "customer": "cu-1", "sku": "EBOOK-FIT" }""",
            Quote);

        Assert.EndsWith(
            "  quote cu-1 (AU, standard, 0.00 kg)\n  subtotal A$34.99\n  add-ons A$0.00\n  shipping A$5.00\n  tax A$1.00\n  total A$40.99\n",
            transcript);
    }

    [Fact]
    public void Quote_WithAPromo_TaxesTheDiscountedAmount()
    {
        var transcript = RunSteps(Ava, TwoShoes, Promo("SHOES10"), Quote);

        Assert.Contains("  promo SHOES10 applied (SPEND>100 AND CATEGORY=Shoes)\n", transcript);
        Assert.EndsWith(
            "  quote cu-1 (AU, standard, 1.60 kg)\n  subtotal A$179.90\n  discount -A$17.99 (SHOES10)\n  add-ons A$0.00\n  shipping A$8.20\n  tax A$16.19\n  total A$186.30\n",
            transcript);
    }

    [Fact]
    public void Quote_WithAPromoAcrossBundleLines_SpreadsTheDiscountOverEveryLine()
    {
        var transcript = RunSteps(
            Ava,
            TwoShoes,
            RunKit,
            Promo("SHOES10"),
            """{ "op": "cart.shipping", "customer": "cu-1", "method": "express" }""",
            """{ "op": "cart.addon", "customer": "cu-1", "addon": "giftwrap" }""",
            """{ "op": "cart.addon", "customer": "cu-1", "addon": "insurance" }""",
            Quote);

        Assert.EndsWith(
            "  quote cu-1 (AU, express, 2.50 kg)\n  subtotal A$271.66\n  discount -A$27.17 (SHOES10)\n  add-ons A$9.39\n  shipping A$22.00\n  tax A$24.45\n  total A$300.33\n",
            transcript);
    }

    [Fact]
    public void CartPromo_WhenTheRuleDoesNotHold_IsRejected()
    {
        var transcript = RunSteps(Ava, TwoShoes, Promo("BIGSPEND"), Promo("NZFRIEND"), Promo("NOPE"));

        Assert.Contains("  rejected: promo BIGSPEND does not apply to this cart\n", transcript);
        Assert.Contains("  rejected: promo NZFRIEND does not apply to this cart\n", transcript);
        Assert.EndsWith("  rejected: unknown promo NOPE\n", transcript);
    }

    [Fact]
    public void CartPromo_ForNzCustomerWithMoreThanTwoItems_IsApplied()
    {
        var transcript = RunSteps(
            Ben,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL", "qty": 3 }""",
            Promo("NZFRIEND"));

        Assert.EndsWith("  promo NZFRIEND applied (REGION=NZ AND QTY>2)\n", transcript);
    }

    [Fact]
    public void CartPromo_ForNzCustomerWithTwoItems_IsRejected()
    {
        var transcript = RunSteps(
            Ben,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL", "qty": 2 }""",
            Promo("NZFRIEND"));

        Assert.EndsWith("  rejected: promo NZFRIEND does not apply to this cart\n", transcript);
    }

    [Theory]
    [InlineData("standard", "A$6.60")]
    [InlineData("express", "A$15.20")]
    [InlineData("pickup", "A$0.00")]
    public void Quote_ForEachShippingMethod_CostsWhatTheMethodCharges(string method, string expected)
    {
        var transcript = RunSteps(
            Ava,
            OneShoe,
            """{ "op": "cart.shipping", "customer": "cu-1", "method": "%METHOD%" }""".Replace("%METHOD%", method),
            Quote);

        Assert.Contains("  shipping " + expected + "\n", transcript);
    }

    [Fact]
    public void CartShipping_WithAnUnknownMethod_IsRejected()
    {
        var transcript = RunSteps(Ava, OneShoe, """{ "op": "cart.shipping", "customer": "cu-1", "method": "drone" }""");

        Assert.EndsWith("  rejected: unknown shipping method drone\n", transcript);
    }

    [Fact]
    public void Quote_WithBothAddOns_AddsGiftWrapAndTwoPercentInsurance()
    {
        var transcript = RunSteps(Ava, OneShoe, GiftWrap, Insurance, Quote);

        Assert.Contains("  add-ons A$6.30\n", transcript);
        Assert.EndsWith("  total A$111.85\n", transcript);
    }

    [Fact]
    public void CartAddOn_WithAnUnknownAddOn_IsRejected()
    {
        var transcript = RunSteps(Ava, """{ "op": "cart.addon", "customer": "cu-1", "addon": "sparkles" }""");

        Assert.EndsWith("  rejected: unknown add-on sparkles\n", transcript);
    }

    [Fact]
    public void CartUndo_RestoresTheAddOnsAsTheyWereBeforeTheChange()
    {
        var transcript = RunSteps(
            Ava,
            TwoShoes,
            GiftWrap,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "MUG-ENAMEL" }""",
            Insurance,
            """{ "op": "cart.undo", "customer": "cu-1" }""",
            Quote);

        Assert.Contains("  subtotal A$179.90\n", transcript);
        Assert.Contains("  add-ons A$4.50\n", transcript);
    }

    [Fact]
    public void Quote_ForAnEmptyCart_IsRejected()
    {
        var transcript = RunSteps(Ava, Quote);

        Assert.EndsWith("  rejected: cart is empty\n", transcript);
    }

    private const string GiftWrap = """{ "op": "cart.addon", "customer": "cu-1", "addon": "giftwrap" }""";
    private const string Insurance = """{ "op": "cart.addon", "customer": "cu-1", "addon": "insurance" }""";

    private static string Promo(string code)
    {
        return """{ "op": "cart.promo", "customer": "cu-1", "code": "%CODE%" }""".Replace("%CODE%", code);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "pricing", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}
