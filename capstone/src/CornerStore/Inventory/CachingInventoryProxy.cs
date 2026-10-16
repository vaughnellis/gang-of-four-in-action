namespace CornerStore;

internal sealed class CachingInventoryProxy(
    IInventoryService inner,
    IBackOfficeInventory backOfficeInventory,
    IStockRestorer stockRestorer)
    : IInventoryService, IBackOfficeInventory, IStockRestorer
{
    private readonly Dictionary<string, int> stockCache = new(StringComparer.Ordinal);

    public int GetStock(string sku)
    {
        if (!stockCache.TryGetValue(sku, out var available))
        {
            available = inner.GetStock(sku);
            stockCache[sku] = available;
        }

        return available;
    }

    public void RestoreStock(string sku, int quantity)
    {
        stockRestorer.RestoreStock(sku, quantity);
        stockCache.Remove(sku);
    }

    public bool ReserveStock(string sku, int quantity)
    {
        var reserved = inner.ReserveStock(sku, quantity);
        if (reserved)
        {
            stockCache.Remove(sku);
        }

        return reserved;
    }

    public bool TryGetStaffStock(Customer actor, string sku, out int available)
    {
        available = GetStock(sku);
        return true;
    }

    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
    {
        var adjusted = backOfficeInventory.TryAdjustStaffStock(actor, sku, adjustment, out newStock);
        if (adjusted)
        {
            stockCache[sku] = newStock;
        }

        return adjusted;
    }
}
