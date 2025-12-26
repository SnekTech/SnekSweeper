namespace SnekSweeper.Commands;

public class CommandInvoker
{
    readonly Stack<ICommand> _undoStack = new();

    public async Task ExecuteCommandAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        await command.ExecuteAsync(cancellationToken);
        _undoStack.Push(command);
    }

    public async Task UndoCommandAsync()
    {
        if (_undoStack.Count == 0)
            return;

        var activeCommand = _undoStack.Pop();
        await activeCommand.UndoAsync();
    }
}