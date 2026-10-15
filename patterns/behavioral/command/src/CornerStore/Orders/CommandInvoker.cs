namespace CornerStore;

internal sealed class CommandInvoker
{
    private readonly List<CommandAuditEntry> auditLog = [];
    private ICommand? lastCommand;

    public IReadOnlyList<CommandAuditEntry> AuditLog => auditLog.AsReadOnly();

    public string Execute(ICommand command)
    {
        var result = command.Execute();
        auditLog.Add(new CommandAuditEntry(command.Name, result));
        lastCommand = command;
        return result;
    }

    public void UndoLast()
    {
        if (lastCommand is null)
        {
            throw new InvalidOperationException("No command is available to undo.");
        }

        if (!lastCommand.CanUndo)
        {
            throw new InvalidOperationException("The most recently executed command cannot be undone.");
        }

        var command = lastCommand;
        command.Undo();
        auditLog.Add(new CommandAuditEntry(command.Name, "undone"));
        lastCommand = null;
    }
}
