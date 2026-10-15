namespace CornerStore;

internal interface ICommand
{
    string Name { get; }

    bool CanUndo { get; }

    string Execute();

    void Undo();
}
