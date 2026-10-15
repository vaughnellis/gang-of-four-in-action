namespace CornerStore;

internal sealed class InventoryOrderStatusObserver(InventoryService inventory) : IOrderStatusObserver
{
    public void OnStatusChanged(Order order)
    {
        foreach (var line in order.Lines)
        {
            inventory.GetStock(line.Product.Sku);
        }
    }
}