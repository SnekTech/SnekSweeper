using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class CompoundCommand : ICommand
{
    readonly IReadOnlyList<ICommand> _commands;

    public CompoundCommand(IEnumerable<ICommand> commands)
    {
        _commands = commands.ToList();
        Name = string.Join(" + ", _commands.Select(command => command.Name));
    }

    public string Name { get; }

    public void Execute(Grid grid)
    {
        foreach (var command in _commands) command.Execute(grid);
    }

    public void Undo(Grid grid)
    {
        foreach (var command in _commands) command.Undo(grid);
    }
}
