using System.Reflection;

namespace CornerStore.Tests;

public class CheckoutFacadeTests
{
    [Fact]
    public void GivenValidCart_PlaceOrder_PaysReservesBooksAndConfirms()
    {
        var log = Log.Instance;
        var output = new StringWriter();
        log.SetOutput(output);

        try
        {
            var fixture = CreateFixture();
            var result = fixture.Facade.PlaceOrder(CreateCart(), CreateCustomer(), "card");
            var order = Assert.Single(fixture.OrderService.GetAll());

            Assert.Equal("checkout placed o-1001", result);
            Assert.Equal("Paid", order.Status);
            Assert.True(order.StockReserved);
            Assert.Equal("SF-0001", order.TrackingNumber);
            Assert.Equal(9, fixture.Inventory.GetStock("MUG-ENAMEL"));
            Assert.Contains("card charged", output.ToString());
            Assert.Contains("ShipFast booked SF-0001 for o-1001", output.ToString());
            Assert.Contains("email to Ava: order o-1001 confirmed", output.ToString());
        }
        finally
        {
            log.ResetOutput();
        }
    }

    [Fact]
    public void GivenMissingAddress_PlaceOrder_DoesNotChargeOrReserveStock()
    {
        var log = Log.Instance;
        var output = new StringWriter();
        log.SetOutput(output);

        try
        {
            var fixture = CreateFixture();
            var stockBeforeCheckout = fixture.Inventory.GetStock("MUG-ENAMEL");
            var customer = CreateCustomer("");

            var result = fixture.Facade.PlaceOrder(CreateCart(), customer, "card");

            Assert.Equal("checkout rejected: address required", result);
            Assert.Empty(fixture.OrderService.GetAll());
            Assert.Equal(stockBeforeCheckout, fixture.Inventory.GetStock("MUG-ENAMEL"));
            Assert.DoesNotContain("card charged", output.ToString());
        }
        finally
        {
            log.ResetOutput();
        }
    }

    [Fact]
    public void GivenConsoleCheckout_UsesCheckoutFacade()
    {
        var facadeField = typeof(StoreConsole).GetField("checkoutFacade", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(facadeField);
        Assert.Equal(typeof(CheckoutFacade), facadeField.FieldType);
    }

    private static Cart CreateCart()
    {
        var cart = new Cart("cu-1");
        cart.Lines.Add(new CartLine
        {
            Product = new PhysicalProduct
            {
                Sku = "MUG-ENAMEL",
                Name = "Enamel Mug",
                CategoryName = "Home",
                Price = 10m,
                WeightKg = 0.3m,
            },
            Quantity = 1,
            UnitPrice = 10m,
        });
        return cart;
    }

    private static Customer CreateCustomer(string address = "1 Harbour St")
    {
        return new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = address,
            CreditLimit = 500m,
        };
    }

    private static FacadeFixture CreateFixture()
    {
        var inventory = new InventoryService();
        var ids = new IdSequence();
        var orderService = new OrderService(
            ids,
            inventory,
            new OrderConfirmedEmail(new EmailSender()),
            new OrderConfirmedSms(new SmsSender()),
            new OrderShippedEmail(new EmailSender()),
            new OrderShippedSms(new SmsSender()),
            new LoyaltyService(),
            new AnalyticsService());
        var facade = new CheckoutFacade(
            new PricingService(new PromoEngine()),
            new OrderValidator(inventory, new FraudScorer()),
            orderService,
            inventory,
            new ShipFastAdapter(new ShipFastClient(ids)));

        return new FacadeFixture(facade, orderService, inventory);
    }

    private sealed record FacadeFixture(
        CheckoutFacade Facade,
        OrderService OrderService,
        InventoryService Inventory);
}