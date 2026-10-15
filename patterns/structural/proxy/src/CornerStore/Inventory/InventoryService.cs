namespace CornerStore;

internal class InventoryService : IInventoryService, IBackOfficeInventory, IStockRestorer
{
    private readonly Dictionary<string, int> stock = new(StringComparer.Ordinal)
    {
        ["SHOE-RUN-42"] = 8,
        ["SHOE-CAS-40"] = 8,
        ["SHOE-KID-30"] = 5,
        ["SOCK-WOOL"] = 12,
        ["LAMP-DESK"] = 4,
        ["TOWEL-SET"] = 8,
        ["MUG-ENAMEL"] = 10,
        ["KETTLE-1L"] = 4,
        ["EBOOK-FIT"] = 100,
        ["EBOOK-GARDEN"] = 100,
        ["GC-25"] = 20,
        ["GC-50"] = 20,
    };

    private int lookupCost;

    internal int LookupCost => lookupCost;

    public int GetStock(string sku)
    {
        lookupCost += 10;
        return stock.GetValueOrDefault(sku);
    }

    public bool ReserveStock(string sku, int quantity)
    {
        var available = GetStock(sku);
        if (available < quantity)
        {
            return false;
        }

        stock[sku] = available - quantity;
        return true;
    }

    public void RestoreStock(string sku, int quantity)
    {
        stock[sku] = GetStock(sku) + quantity;
    }

    public bool TryGetStaffStock(Customer actor, string sku, out int available)
    {
        available = GetStock(sku);
        return true;
    }

    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
    {
        newStock = GetStock(sku) + adjustment;
        stock[sku] = newStock;
        return true;
    }
}