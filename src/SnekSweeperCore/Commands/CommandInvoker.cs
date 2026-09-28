using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class CommandInvoker : ICommandRecorder
{
    readonly Stack<ICommand> _undoStack = new();

    public void ExecuteAndRecord(Grid grid, ICommand command)
    {
        command.Execute(grid);
        _undoStack.Push(command);
    }

    public void UndoCommand(Grid grid)
    {
        if (_undoStack.Count == 0) return;

        _undoStack.Pop().Undo(grid);
    }
}
