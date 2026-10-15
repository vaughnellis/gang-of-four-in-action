using System.Globalization;

namespace CornerStore;

internal sealed class AdjustStockCommand(InventoryService inventory, string sku, int adjustment) : ICommand
{
    private bool canUndo;

    public string Name => "AdjustStock";

    public bool CanUndo => canUndo;

    public string Execute()
    {
        if (canUndo)
        {
            throw new InvalidOperationException("Undo the previous execution before reusing this command.");
        }

        var stock = inventory.AdjustStock(sku, adjustment);
        canUndo = true;
        return "stock " + sku + " adjusted by " + adjustment.ToString(CultureInfo.InvariantCulture)
            + ", now " + stock.ToString(CultureInfo.InvariantCulture);
    }

    public void Undo()
    {
        if (!canUndo)
        {
            throw new InvalidOperationException("This stock adjustment cannot be undone.");
        }

        inventory.AdjustStock(sku, -adjustment);
        canUndo = false;
    }
}
