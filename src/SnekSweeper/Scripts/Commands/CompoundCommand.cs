using System.Text;

namespace SnekSweeper.Commands;

public class CompoundCommand : ICommand
{
    readonly IList<ICommand> _commands;

    public CompoundCommand(IEnumerable<ICommand> commands)
    {
        _commands = commands.ToList();
        Name = CalculateCommandName();
    }

    public string Name { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var executeTasks = _commands.Select(command => command.ExecuteAsync(cancellationToken)).ToList();
        return Task.WhenAll(executeTasks);
    }

    public Task UndoAsync()
    {
        var undoTasks = _commands.Select(command => command.UndoAsync()).ToList();
        return Task.WhenAll(undoTasks);
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