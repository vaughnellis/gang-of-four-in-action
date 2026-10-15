namespace CornerStore.Tests;

public class MediatorTests
{
    [Fact]
    public void ItemListChange_RefreshesWidgetsInTheEstablishedOrder()
    {
        var mediator = CreateMediator();

        var output = Capture(() => Assert.Null(mediator.Add("SHOE-RUN-42", 1)));

        Assert.Equal(
            [
                "  - SHOE-RUN-42 Runner Pro x1 @ A$89.95 = A$89.95",
                "  promo none",
                "  shipping standard",
                "  totals A$105.55",
            ],
            output);
    }

    [Fact]
    public void PromoChange_RefreshesWidgetsInTheEstablishedOrder()
    {
        var mediator = CreateMediator();
        Assert.Null(mediator.Add("SHOE-RUN-42", 2));

        var output = Capture(() => mediator.ApplyPromo("SHOES10"));

        Assert.Equal(
            [
                "  promo SHOES10",
                "  - SHOE-RUN-42 Runner Pro x2 @ A$89.95 = A$179.90",
                "  shipping standard",
                "  totals A$186.30",
            ],
            output);
    }

    [Fact]
    public void ShippingChange_RefreshesWidgetsInTheEstablishedOrder()
    {
        var mediator = CreateMediator();
        Assert.Null(mediator.Add("SHOE-RUN-42", 1));

        var output = Capture(() => mediator.SelectShipping("express"));

        Assert.Equal(
            [
                "  shipping express",
                "  - SHOE-RUN-42 Runner Pro x1 @ A$89.95 = A$89.95",
                "  promo none",
                "  totals A$114.15",
            ],
            output);
    }

    [Fact]
    public void TotalsChange_RefreshesWidgetsInTheEstablishedOrder()
    {
        var mediator = CreateMediator();

        var output = Capture(mediator.TotalsChanged);

        Assert.Equal(
            [
                "  totals A$0.00",
                "  promo none",
                "  shipping standard",
            ],
            output);
    }

    private static CartMediator CreateMediator()
    {
        var cart = new Cart("cu-1");
        var customer = new Customer { Id = "cu-1", Name = "Ava", Region = "AU", CreditLimit = 800m };
        var promos = new PromoEngine();

        return new CartMediator(cart, customer, new CartService(new Catalog()), promos, new PricingService(promos));
    }

    private static string[] Capture(Action action)
    {
        var originalOutput = Log.Out;
        using var output = new StringWriter();
        try
        {
            Log.Out = output;
            action();
            return output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }
        finally
        {
            Log.Out = originalOutput;
        }
    }
}
