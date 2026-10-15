namespace CornerStore.Tests;

public class CartScenarioTests : IDisposable
{
    private const string Ava = """{ "op": "register", "name": "Ava", "region": "AU", "channel": "email", "creditLimit": 800 }""";
    private const string TwoShoes = """{ "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42", "qty": 2 }""";
    private const string RunKit = """{ "op": "cart.add", "customer": "cu-1", "sku": "BUN-RUN-KIT" }""";
    private const string Undo = """{ "op": "cart.undo", "customer": "cu-1" }""";

    private readonly string _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Register_AssignsSequentialCustomerIds()
    {
        var transcript = RunSteps(Ava, """{ "op": "register", "name": "Ben", "region": "NZ", "channel": "sms" }""");

        Assert.Contains("  registered cu-1 Ava (AU, email)\n", transcript);
        Assert.Contains("  registered cu-2 Ben (NZ, sms)\n", transcript);
    }

    [Fact]
    public void CartAdd_OfABundle_ExpandsIntoDiscountedLinesThatAddUpToTheBundlePrice()
    {
        var transcript = RunSteps(Ava, TwoShoes, RunKit, """{ "op": "cart.show", "customer": "cu-1" }""");

        Assert.Contains("  - SHOE-RUN-42 Runner Pro x2 @ A$89.95 = A$179.90\n", transcript);
        Assert.Contains("  - SHOE-RUN-42 Runner Pro x1 @ A$80.96 = A$80.96 (from BUN-RUN-KIT)\n", transcript);
        Assert.Contains("  - SOCK-WOOL Wool Socks x1 @ A$10.80 = A$10.80 (from BUN-RUN-KIT)\n", transcript);
        Assert.EndsWith("  cart cu-1 lines=3 subtotal=A$271.66\n", transcript);
    }

    [Fact]
    public void CartAdd_RefreshesWidgetsInTheirDirectCallOrder()
    {
        var transcript = RunSteps(Ava, TwoShoes);

        var itemList = transcript.IndexOf("  - SHOE-RUN-42 Runner Pro x2 @ A$89.95 = A$179.90\n", StringComparison.Ordinal);
        var promoBox = transcript.IndexOf("  promo none\n", StringComparison.Ordinal);
        var shippingSelector = transcript.IndexOf("  shipping standard\n", StringComparison.Ordinal);
        var totalsPanel = transcript.IndexOf("  totals A$206.09\n", StringComparison.Ordinal);

        Assert.True(itemList >= 0 && itemList < promoBox);
        Assert.True(promoBox < shippingSelector);
        Assert.True(shippingSelector < totalsPanel);
    }

    [Fact]
    public void CartUndo_AfterAnAdd_RemovesTheAddedLines()
    {
        var transcript = RunSteps(Ava, TwoShoes, RunKit, Undo);

        Assert.EndsWith("  undone\n  cart cu-1 lines=1 subtotal=A$179.90\n", transcript);
    }

    [Fact]
    public void CartUndo_AfterARemove_RestoresTheRemovedLineWithItsQuantity()
    {
        var transcript = RunSteps(Ava, TwoShoes, """{ "op": "cart.remove", "customer": "cu-1", "sku": "SHOE-RUN-42" }""", Undo);

        Assert.Contains("  removed SHOE-RUN-42\n  cart cu-1 lines=0 subtotal=A$0.00\n", transcript);
        Assert.EndsWith("  undone\n  cart cu-1 lines=1 subtotal=A$179.90\n", transcript);
    }

    [Fact]
    public void CartUndo_Twice_OnlyUndoesOneLevel()
    {
        var transcript = RunSteps(Ava, TwoShoes, RunKit, Undo, Undo);

        Assert.EndsWith("  nothing to undo\n", transcript);
    }

    [Fact]
    public void CartAdd_WhenRejected_KeepsTheEarlierUndoState()
    {
        var transcript = RunSteps(
            Ava,
            TwoShoes,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "NOPE-1" }""",
            Undo);

        Assert.Contains("  rejected: unknown sku NOPE-1\n", transcript);
        Assert.EndsWith("  undone\n  cart cu-1 lines=0 subtotal=A$0.00\n", transcript);
    }

    [Fact]
    public void CartRemove_OfAPlainSku_LeavesTheBundleComponentLines()
    {
        var transcript = RunSteps(
            Ava,
            RunKit,
            """{ "op": "cart.add", "customer": "cu-1", "sku": "SHOE-RUN-42" }""",
            """{ "op": "cart.remove", "customer": "cu-1", "sku": "SHOE-RUN-42" }""");

        Assert.EndsWith("  removed SHOE-RUN-42\n  cart cu-1 lines=2 subtotal=A$91.76\n", transcript);
    }

    [Fact]
    public void CartAdd_ForAnUnknownCustomer_IsRejected()
    {
        var transcript = RunSteps(Ava, """{ "op": "cart.add", "customer": "cu-9", "sku": "MUG-ENAMEL" }""");

        Assert.EndsWith("  rejected: unknown customer cu-9\n", transcript);
    }

    private string RunSteps(params string[] steps)
    {
        File.WriteAllText(
            _path,
            """{ "name": "cart", "startTime": "2026-03-02T09:00:00Z", "seed": 1, "steps": [""" + string.Join(",", steps) + "] }");
        var output = new StringWriter();

        ScenarioRunner.Run(_path, output);

        return output.ToString();
    }
}
