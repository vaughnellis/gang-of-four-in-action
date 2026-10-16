using System.Globalization;

namespace CornerStore;

internal sealed class AdjustStockCommand(
    IBackOfficeInventory inventory,
    Customer actor,
    string sku,
    int adjustment) : ICommand
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

        if (!inventory.TryAdjustStaffStock(actor, sku, adjustment, out var stock))
        {
            return "rejected: staff only";
        }

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

        if (!inventory.TryAdjustStaffStock(actor, sku, -adjustment, out _))
        {
            throw new InvalidOperationException("The stock adjustment could not be undone.");
        }

        canUndo = false;
    }
}
