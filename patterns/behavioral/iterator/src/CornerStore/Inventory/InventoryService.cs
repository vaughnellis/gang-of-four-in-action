namespace CornerStore;

internal class InventoryService
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

    // SMELL S12: each uncached stock lookup adds simulated latency cost
    public int GetStock(string sku)
    {
        lookupCost += 10;
        return stock.GetValueOrDefault(sku);
    }

    // SMELL S12: callers can change stock without an authorization check
    public int AdjustStock(string sku, int adjustment)
    {
        stock[sku] = GetStock(sku) + adjustment;
        return stock[sku];
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
}