using System.Collections.Generic;
using Godot;

namespace SnekSweeper.Commands;

public class CommandInvoker
{
    private readonly Stack<ICommand> _undoStack = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        GD.Print($"execute: {command.Name}");
        _undoStack.Push(command);
    }

    public void UndoCommand()
    {
        if (_undoStack.Count <= 0) return;

        var activeCommand = _undoStack.Pop();
        GD.Print($"undo: {activeCommand.Name}");
        activeCommand.Undo();
    }
}