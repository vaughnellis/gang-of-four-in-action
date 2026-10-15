namespace CornerStore;

internal sealed class ProtectionInventoryProxy(
    IInventoryService inner,
    IBackOfficeInventory backOfficeInventory,
    IStockRestorer stockRestorer)
    : IInventoryService, IBackOfficeInventory, IStockRestorer
{
    public int GetStock(string sku)
    {
        return inner.GetStock(sku);
    }

    public void RestoreStock(string sku, int quantity)
    {
        stockRestorer.RestoreStock(sku, quantity);
    }

    public bool ReserveStock(string sku, int quantity)
    {
        return inner.ReserveStock(sku, quantity);
    }

    public bool TryGetStaffStock(Customer actor, string sku, out int available)
    {
        if (actor.Role != "staff")
        {
            available = 0;
            return false;
        }

        return backOfficeInventory.TryGetStaffStock(actor, sku, out available);
    }

    public bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock)
    {
        if (actor.Role != "staff")
        {
            newStock = 0;
            return false;
        }

        return backOfficeInventory.TryAdjustStaffStock(actor, sku, adjustment, out newStock);
    }
}
