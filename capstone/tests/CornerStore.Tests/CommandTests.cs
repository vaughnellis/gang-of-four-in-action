namespace CornerStore.Tests;

public class CommandTests
{
    [Fact]
    public void GivenStaff_WhenAdjustingStock_RecordsResultAndCanUndo()
    {
        var fixture = CreateFixture();
        var staff = CreateCustomer("staff");
        var initialStock = fixture.Inventory.GetStock("MUG-ENAMEL");

        var result = fixture.BackOffice.AdjustStock(staff, "MUG-ENAMEL", 3);

        Assert.Equal("stock MUG-ENAMEL adjusted by 3, now 13", result);
        Assert.Equal(new CommandAuditEntry("AdjustStock", result), Assert.Single(fixture.BackOffice.AuditLog));

        fixture.BackOffice.UndoLastAction();

        Assert.Equal(initialStock, fixture.Inventory.GetStock("MUG-ENAMEL"));
        Assert.Equal(new CommandAuditEntry("AdjustStock", "undone"), fixture.BackOffice.AuditLog[^1]);
    }

    [Fact]
    public void GivenCustomer_WhenAdjustingStock_ReturnsRejectionWithoutAuditing()
    {
        var fixture = CreateFixture();
        var customer = CreateCustomer("customer");
        var initialStock = fixture.Inventory.GetStock("MUG-ENAMEL");

        var result = fixture.BackOffice.AdjustStock(customer, "MUG-ENAMEL", 3);

        Assert.Equal("rejected: staff only", result);
        Assert.Equal(initialStock, fixture.Inventory.GetStock("MUG-ENAMEL"));
        Assert.Empty(fixture.BackOffice.AuditLog);
    }

    [Fact]
    public void GivenStockLookup_WhenExecuted_RecordsResultAndDoesNotCreateUndo()
    {
        var fixture = CreateFixture();
        var staff = CreateCustomer("staff");

        var result = fixture.BackOffice.LookupStock(staff, "MUG-ENAMEL");

        Assert.Equal("stock MUG-ENAMEL 10", result);
        Assert.Equal(new CommandAuditEntry("LookupStock", result), Assert.Single(fixture.BackOffice.AuditLog));
        Assert.Throws<InvalidOperationException>(fixture.BackOffice.UndoLastAction);
    }

    private static BackOfficeFixture CreateFixture()
    {
        var inventory = new InventoryService();
        var cachingProxy = new CachingInventoryProxy(inventory, inventory, inventory);
        var protectionProxy = new ProtectionInventoryProxy(cachingProxy, cachingProxy, cachingProxy);
        var orderService = new OrderService(new IdSequence(), protectionProxy);
        return new BackOfficeFixture(new BackOffice(protectionProxy, orderService), inventory);
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

    private sealed record BackOfficeFixture(BackOffice BackOffice, InventoryService Inventory);
}
