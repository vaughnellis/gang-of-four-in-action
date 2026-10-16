namespace CornerStore.Tests;

public class ProxyTests
{
    [Fact]
    public void CachingProxy_WhenReadingSkuTwice_UsesOneUnderlyingLookup()
    {
        var service = new InventoryService();
        var proxy = new CachingInventoryProxy(service, service, service);

        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
        Assert.Equal(10, service.LookupCost);
    }

    [Fact]
    public void CachingProxy_WhenStockIsAdjusted_UpdatesCachedValue()
    {
        var service = new InventoryService();
        var proxy = new CachingInventoryProxy(service, service, service);
        var staff = new Customer { Role = "staff" };

        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
        Assert.True(proxy.TryAdjustStaffStock(staff, "SHOE-RUN-42", 2, out var newStock));
        Assert.Equal(10, newStock);
        Assert.Equal(10, proxy.GetStock("SHOE-RUN-42"));
        Assert.Equal(20, service.LookupCost);
    }

    [Fact]
    public void CachingProxy_WhenStockIsReserved_InvalidatesCachedValue()
    {
        var service = new InventoryService();
        var proxy = new CachingInventoryProxy(service, service, service);

        Assert.Equal(8, proxy.GetStock("SHOE-RUN-42"));
        Assert.True(proxy.ReserveStock("SHOE-RUN-42", 3));
        Assert.Equal(5, proxy.GetStock("SHOE-RUN-42"));
        Assert.Equal(30, service.LookupCost);
    }

    [Fact]
    public void ProtectionProxy_WhenActorIsNotStaff_RejectsStockOperations()
    {
        var service = new InventoryService();
        var cache = new CachingInventoryProxy(service, service, service);
        var proxy = new ProtectionInventoryProxy(cache, cache, cache);
        var customer = new Customer { Role = "customer" };

        Assert.False(proxy.TryGetStaffStock(customer, "SHOE-RUN-42", out var available));
        Assert.Equal(0, available);
        Assert.False(proxy.TryAdjustStaffStock(customer, "SHOE-RUN-42", 2, out var newStock));
        Assert.Equal(0, newStock);
        Assert.Equal(0, service.LookupCost);
    }

    [Fact]
    public void ProtectionProxy_WhenActorIsStaff_AllowsStockOperations()
    {
        var service = new InventoryService();
        var cache = new CachingInventoryProxy(service, service, service);
        var proxy = new ProtectionInventoryProxy(cache, cache, cache);
        var staff = new Customer { Role = "staff" };

        Assert.True(proxy.TryGetStaffStock(staff, "SHOE-RUN-42", out var available));
        Assert.Equal(8, available);
        Assert.True(proxy.TryAdjustStaffStock(staff, "SHOE-RUN-42", 2, out var newStock));
        Assert.Equal(10, newStock);
        Assert.True(proxy.TryGetStaffStock(staff, "SHOE-RUN-42", out var updatedStock));
        Assert.Equal(10, updatedStock);
        Assert.Equal(20, service.LookupCost);
    }
}
