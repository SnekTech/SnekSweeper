using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace SnekSweeper.Commands;

public class CommandInvoker
{
    private readonly Stack<ICommand> _undoStack = new();

    public async Task ExecuteCommandAsync(ICommand command)
    {
        await command.ExecuteAsync();
        GD.Print($"execute: {command.Name}");
        _undoStack.Push(command);
    }

    public async Task UndoCommandAsync()
    {
        if (_undoStack.Count == 0)
            return;

        var activeCommand = _undoStack.Pop();
        GD.Print($"undo: {activeCommand.Name}");
        await activeCommand.UndoAsync();
    }
}