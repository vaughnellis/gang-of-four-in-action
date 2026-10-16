namespace CornerStore.Tests;

public class StateTests
{
    [Fact]
    public void GivenPendingOrder_ValidTransitions_ReachRefunded()
    {
        var fixture = CreateFixture();

        Assert.Null(fixture.OrderService.Pay(fixture.Order));
        Assert.Equal("Paid", fixture.Order.Status);
        Assert.Null(fixture.OrderService.Ship(fixture.Order));
        Assert.Equal("Shipped", fixture.Order.Status);
        Assert.Null(fixture.OrderService.Deliver(fixture.Order));
        Assert.Equal("Delivered", fixture.Order.Status);
        Assert.Null(fixture.OrderService.Refund(fixture.Order));
        Assert.Equal("Refunded", fixture.Order.Status);
    }

    [Fact]
    public void GivenPendingOrder_Ship_IsRejectedWithoutChangingState()
    {
        var fixture = CreateFixture();

        var error = fixture.OrderService.Ship(fixture.Order);

        Assert.Equal("cannot ship from Pending", error);
        Assert.Equal("Pending", fixture.Order.Status);
    }

    [Fact]
    public void GivenReservedStock_Cancel_RestoresInventory()
    {
        var fixture = CreateFixture(withLine: true);
        var stockBeforeReservation = fixture.Inventory.GetStock("MUG-ENAMEL");
        Assert.True(fixture.Inventory.ReserveStock("MUG-ENAMEL", 1));
        fixture.Order.StockReserved = true;

        var error = fixture.OrderService.Cancel(fixture.Order);

        Assert.Null(error);
        Assert.Equal("Cancelled", fixture.Order.Status);
        Assert.Equal(stockBeforeReservation, fixture.Inventory.GetStock("MUG-ENAMEL"));
    }

    [Fact]
    public void GivenPaidOrderWithReservedStock_Refund_RestoresInventory()
    {
        var fixture = CreateFixture(withLine: true);
        Assert.Null(fixture.OrderService.Pay(fixture.Order));
        var stockBeforeReservation = fixture.Inventory.GetStock("MUG-ENAMEL");
        Assert.True(fixture.Inventory.ReserveStock("MUG-ENAMEL", 1));
        fixture.Order.StockReserved = true;

        var error = fixture.OrderService.Refund(fixture.Order);

        Assert.Null(error);
        Assert.Equal("Refunded", fixture.Order.Status);
        Assert.Equal(stockBeforeReservation, fixture.Inventory.GetStock("MUG-ENAMEL"));
    }

    private static OrderFixture CreateFixture(bool withLine = false)
    {
        var inventory = new InventoryService();
        var ids = new IdSequence();
        var emailSender = new EmailSender();
        var smsSender = new SmsSender();
        var orderService = new OrderService(ids, inventory);
        orderService.Subscribe(new InventoryOrderStatusObserver(inventory));
        orderService.Subscribe(new NotificationOrderStatusObserver(emailSender, smsSender));
        orderService.Subscribe(new LoyaltyOrderStatusObserver(new LoyaltyService()));
        orderService.Subscribe(new AnalyticsOrderStatusObserver(new AnalyticsService()));
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
        if (withLine)
        {
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
        }

        var order = orderService.Create(customer, cart, new Quote { Total = 10m }, "card");
        return new OrderFixture(orderService, inventory, order);
    }

    private sealed record OrderFixture(OrderService OrderService, InventoryService Inventory, Order Order);
}
