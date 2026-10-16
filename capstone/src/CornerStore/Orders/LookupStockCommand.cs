using System.Globalization;

namespace CornerStore;

internal sealed class LookupStockCommand(IBackOfficeInventory inventory, Customer actor, string sku) : ICommand
{
    public string Name => "LookupStock";

    public bool CanUndo => false;

    public string Execute()
    {
        if (!inventory.TryGetStaffStock(actor, sku, out var available)
            || !inventory.TryGetStaffStock(actor, sku, out _))
        {
            return "rejected: staff only";
        }

        return "stock " + sku + " " + available.ToString(CultureInfo.InvariantCulture);
    }

    public void Undo()
    {
        throw new NotSupportedException("A stock lookup has no state to undo.");
    }
}
