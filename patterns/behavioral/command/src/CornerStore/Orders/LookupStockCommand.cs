using System.Globalization;

namespace CornerStore;

internal sealed class LookupStockCommand(InventoryService inventory, string sku) : ICommand
{
    public string Name => "LookupStock";

    public bool CanUndo => false;

    public string Execute()
    {
        var available = inventory.GetStock(sku);
        inventory.GetStock(sku);
        return "stock " + sku + " " + available.ToString(CultureInfo.InvariantCulture);
    }

    public void Undo()
    {
        throw new NotSupportedException("A stock lookup has no state to undo.");
    }
}
