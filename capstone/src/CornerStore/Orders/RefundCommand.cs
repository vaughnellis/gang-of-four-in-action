namespace CornerStore;

internal sealed class RefundCommand(OrderService orderService, Order order) : ICommand
{
    public string Name => "Refund";

    public bool CanUndo => false;

    public string Execute()
    {
        var error = orderService.Refund(order);
        return error is null ? "refunded " + order.Id : "rejected: " + error;
    }

    public void Undo()
    {
        throw new NotSupportedException("A refund has effects that cannot be undone.");
    }
}
