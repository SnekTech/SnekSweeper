using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnekSweeper.Commands;

public class CompoundCommand : ICommand
{
    private readonly IList<ICommand> _commands;

    public CompoundCommand(IEnumerable<ICommand> commands)
    {
        _commands = commands.ToList();
        Name = CalculateCommandName();
    }

    public string Name { get; }

    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public void Undo()
    {
        foreach (var command in _commands)
        {
            command.Undo();
        }
    }

    private string CalculateCommandName()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Compound: [");
        foreach (var command in _commands)
        {
            stringBuilder.Append(command.Name).Append(", ");
        }

        stringBuilder.Append("] command");

        return stringBuilder.ToString();
    }
}