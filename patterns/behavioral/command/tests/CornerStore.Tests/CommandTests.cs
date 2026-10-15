namespace CornerStore.Tests;

public class CommandTests
{
    [Fact]
    public void GivenStaff_WhenAdjustingStock_RecordsResultAndCanUndo()
    {
        var inventory = new InventoryService();
        var backOffice = CreateBackOffice(inventory);
        var staff = CreateCustomer("staff");
        var initialStock = inventory.GetStock("MUG-ENAMEL");

        var result = backOffice.AdjustStock(staff, "MUG-ENAMEL", 3);

        Assert.Equal("stock MUG-ENAMEL adjusted by 3, now 13", result);
        Assert.Equal(new CommandAuditEntry("AdjustStock", result), Assert.Single(backOffice.AuditLog));

        backOffice.UndoLastAction();

        Assert.Equal(initialStock, inventory.GetStock("MUG-ENAMEL"));
        Assert.Equal(new CommandAuditEntry("AdjustStock", "undone"), backOffice.AuditLog[^1]);
    }

    [Fact]
    public void GivenCustomer_WhenAdjustingStock_ReturnsRejectionWithoutAuditing()
    {
        var inventory = new InventoryService();
        var backOffice = CreateBackOffice(inventory);
        var customer = CreateCustomer("customer");
        var initialStock = inventory.GetStock("MUG-ENAMEL");

        var result = backOffice.AdjustStock(customer, "MUG-ENAMEL", 3);

        Assert.Equal("rejected: staff only", result);
        Assert.Equal(initialStock, inventory.GetStock("MUG-ENAMEL"));
        Assert.Empty(backOffice.AuditLog);
    }

    [Fact]
    public void GivenStockLookup_WhenExecuted_RecordsResultAndDoesNotCreateUndo()
    {
        var backOffice = CreateBackOffice(new InventoryService());
        var staff = CreateCustomer("staff");

        var result = backOffice.LookupStock(staff, "MUG-ENAMEL");

        Assert.Equal("stock MUG-ENAMEL 10", result);
        Assert.Equal(new CommandAuditEntry("LookupStock", result), Assert.Single(backOffice.AuditLog));
        Assert.Throws<InvalidOperationException>(backOffice.UndoLastAction);
    }

    private static BackOffice CreateBackOffice(InventoryService inventory)
    {
        var emailSender = new EmailSender();
        var smsSender = new SmsSender();
        var orderService = new OrderService(
            new IdSequence(),
            inventory,
            new OrderConfirmedEmail(emailSender),
            new OrderConfirmedSms(smsSender),
            new OrderShippedEmail(emailSender),
            new OrderShippedSms(smsSender),
            new LoyaltyService(),
            new AnalyticsService());

        return new BackOffice(inventory, orderService);
    }

    private static Customer CreateCustomer(string role)
    {
        return new Customer
        {
            Id = "cu-1",
            Number = 1,
            Name = "Ava",
            Region = "AU",
            Channel = "email",
            Address = "1 Harbour St",
            CreditLimit = 500m,
            Role = role,
        };
    }
}
