namespace CornerStore;

internal interface IInventoryService
{
    int GetStock(string sku);

    bool ReserveStock(string sku, int quantity);
}

internal interface IBackOfficeInventory
{
    bool TryGetStaffStock(Customer actor, string sku, out int available);

    bool TryAdjustStaffStock(Customer actor, string sku, int adjustment, out int newStock);
}

internal interface IStockRestorer
{
    void RestoreStock(string sku, int quantity);
}
