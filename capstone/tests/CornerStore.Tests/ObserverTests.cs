namespace CornerStore.Tests;

public class ObserverTests
{
    [Fact]
    public void GivenRegisteredObservers_Pay_NotifiesEachObserverInOrder()
    {
        var service = new OrderService(new IdSequence(), new InventoryService());
        var firstObserver = new RecordingObserver();
        var secondObserver = new RecordingObserver();
        service.Subscribe(firstObserver);
        service.Subscribe(secondObserver);
        var order = CreateOrder(service);

        var error = service.Pay(order);

        Assert.Null(error);
        Assert.Equal([order], firstObserver.Orders);
        Assert.Equal([order], secondObserver.Orders);
    }

    [Fact]
    public void GivenInvalidTransition_Ship_DoesNotNotifyObservers()
    {
        var service = new OrderService(new IdSequence(), new InventoryService());
        var observer = new RecordingObserver();
        service.Subscribe(observer);
        var order = CreateOrder(service);

        var error = service.Ship(order);

        Assert.Equal("cannot ship from Pending", error);
        Assert.Empty(observer.Orders);
    }

    [Fact]
    public void GivenBuiltInObservers_Pay_PreservesConfirmationAndReportingEffects()
    {
        var log = Log.Instance;
        using var output = new StringWriter();
        log.SetOutput(output);

        try
        {
            var inventory = new InventoryService();
            var emailSender = new EmailSender();
            var smsSender = new SmsSender();
            var service = new OrderService(new IdSequence(), inventory);
            service.Subscribe(new InventoryOrderStatusObserver(inventory));
            service.Subscribe(new NotificationOrderStatusObserver(
                new OrderConfirmedEmail(emailSender),
                new OrderConfirmedSms(smsSender),
                new OrderShippedEmail(emailSender),
                new OrderShippedSms(smsSender)));
            service.Subscribe(new LoyaltyOrderStatusObserver(new LoyaltyService()));
            service.Subscribe(new AnalyticsOrderStatusObserver(new AnalyticsService()));
            var order = CreateOrder(service);

            var error = service.Pay(order);

            Assert.Null(error);
            Assert.Contains("email to Ava: order o-1001 confirmed", output.ToString());
            Assert.Contains("loyalty updated for Ava (Paid)", output.ToString());
            Assert.Contains("analytics order o-1001 Paid", output.ToString());
        }
        finally
        {
            log.ResetOutput();
        }
    }

    private static Order CreateOrder(OrderService service)
    {
        var customer = new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = "1 Harbour St",
            CreditLimit = 500m,
        };
        var cart = new Cart(customer.Id);
        var quote = new Quote { Total = 10m };
        return service.Create(customer, cart, quote, "card");
    }

    private sealed class RecordingObserver : IOrderStatusObserver
    {
        public List<Order> Orders { get; } = [];

        public void OnStatusChanged(Order order)
        {
            Orders.Add(order);
        }
    }
}
