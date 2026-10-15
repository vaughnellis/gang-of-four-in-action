namespace CornerStore;

internal sealed class CancelCommand(OrderService orderService, Order order) : ICommand
{
    public string Name => "Cancel";

    public bool CanUndo => false;

    public string Execute()
    {
        var error = orderService.Cancel(order);
        return error is null ? "cancelled " + order.Id : "rejected: " + error;
    }

    public void Undo()
    {
        throw new NotSupportedException("A cancellation has effects that cannot be undone.");
    }
}
