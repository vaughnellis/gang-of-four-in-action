namespace CornerStore;

internal sealed class StockValidationHandler(InventoryService inventory, OrderValidationHandler? next)
    : OrderValidationHandler(next)
{
    protected override string? Validate(OrderValidationContext context)
    {
        var requestedStock = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var line in context.Cart.Lines)
        {
            requestedStock[line.Product.Sku] = requestedStock.GetValueOrDefault(line.Product.Sku) + line.Quantity;
        }

        foreach (var (sku, quantity) in requestedStock.OrderBy(entry => entry.Key, StringComparer.Ordinal))
        {
            if (inventory.GetStock(sku) < quantity)
            {
                return "out of stock " + sku;
            }
        }

        return null;
    }
}