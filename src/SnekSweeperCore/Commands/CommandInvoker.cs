using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class CommandInvoker : ICommandRecorder
{
    readonly Stack<ICommand> _undoStack = new();

    public GridOutcome ExecuteAndRecord(Grid grid, ICommand command)
    {
        var outcome = command.Execute(grid);
        _undoStack.Push(command);
        return outcome;
    }

    public GridOutcome UndoCommand(Grid grid)
    {
        return _undoStack.Count == 0 ? new GridOutcome.NothingHappens() : _undoStack.Pop().Undo(grid);
    }
}
