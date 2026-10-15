namespace CornerStore;

internal class BackOffice(InventoryService inventory, OrderService orderService)
{
    private readonly CommandInvoker commandInvoker = new();

    public IReadOnlyList<CommandAuditEntry> AuditLog => commandInvoker.AuditLog;

    public void UndoLastAction()
    {
        commandInvoker.UndoLast();
    }

    public string AdjustStock(Customer actor, string sku, int adjustment)
    {
        // SMELL S12: the caller checks the staff role before changing stock
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new AdjustStockCommand(inventory, sku, adjustment));
    }

    public string LookupStock(Customer actor, string sku)
    {
        // SMELL S12: the caller checks the staff role before reading stock
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new LookupStockCommand(inventory, sku));
    }

    public string Refund(Customer actor, Order order)
    {
        // SMELL S12: the caller checks the staff role before refunding
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new RefundCommand(orderService, order));
    }

    public string Cancel(Customer actor, Order order)
    {
        // SMELL S12: the caller checks the staff role before cancelling
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new CancelCommand(orderService, order));
    }
}