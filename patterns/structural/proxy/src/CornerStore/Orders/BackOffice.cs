namespace CornerStore;

internal class BackOffice(IBackOfficeInventory inventory, OrderService orderService)
{
    public string AdjustStock(Customer actor, string sku, int adjustment)
    {
        // SMELL S14: stock adjustment is an inline back-office action
        if (!inventory.TryAdjustStaffStock(actor, sku, adjustment, out var stock))
        {
            return "rejected: staff only";
        }

        return "stock " + sku + " adjusted by " + adjustment.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ", now " + stock.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public string LookupStock(Customer actor, string sku)
    {
        // SMELL S14: stock lookup is an inline back-office action
        if (!inventory.TryGetStaffStock(actor, sku, out var available)
            || !inventory.TryGetStaffStock(actor, sku, out _))
        {
            return "rejected: staff only";
        }

        return "stock " + sku + " " + available.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public string Refund(Customer actor, Order order)
    {
        // SMELL S14: refund runs inline in the back-office service
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        var error = orderService.Refund(order);
        return error is null ? "refunded " + order.Id : "rejected: " + error;
    }

    public string Cancel(Customer actor, Order order)
    {
        // SMELL S14: cancellation runs inline in the back-office service
        if (actor.Role != "staff")
        {
            return "rejected: staff only";
        }

        var error = orderService.Cancel(order);
        return error is null ? "cancelled " + order.Id : "rejected: " + error;
    }
}