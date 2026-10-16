namespace CornerStore;

internal class BackOffice(IBackOfficeInventory inventory, OrderService orderService)
{
    private readonly CommandInvoker commandInvoker = new();

    public IReadOnlyList<CommandAuditEntry> AuditLog => commandInvoker.AuditLog;

    public void UndoLastAction()
    {
        commandInvoker.UndoLast();
    }

    public string AdjustStock(Customer actor, string sku, int adjustment)
    {
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new AdjustStockCommand(inventory, actor, sku, adjustment));
    }

    public string LookupStock(Customer actor, string sku)
    {
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new LookupStockCommand(inventory, actor, sku));
    }

    public string Refund(Customer actor, Order order)
    {
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new RefundCommand(orderService, order));
    }

    public string Cancel(Customer actor, Order order)
    {
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        return commandInvoker.Execute(new CancelCommand(orderService, order));
    }
}