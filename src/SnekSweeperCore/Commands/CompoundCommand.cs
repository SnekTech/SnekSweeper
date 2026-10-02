using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class CompoundCommand : ICommand
{
    readonly IReadOnlyList<ICommand> _commands;

    public CompoundCommand(IEnumerable<ICommand> commands)
    {
        _commands = [.. commands];
        Name = string.Join(" + ", _commands.Select(command => command.Name));
    }

    public string Name { get; }

    public GridOutcome Execute(Grid grid)
    {
        return _commands.Select(command => command.Execute(grid))
            .Aggregate<GridOutcome, GridOutcome>(new GridOutcome.NothingHappens(), (outcomeAcc, nextOutcome) => outcomeAcc.MergeWith(nextOutcome));
    }

    public GridOutcome Undo(Grid grid)
    {
        return _commands.Select(command => command.Undo(grid))
            .Aggregate<GridOutcome, GridOutcome>(new GridOutcome.NothingHappens(), (outcomeAcc, nextOutcome) => outcomeAcc.MergeWith(nextOutcome));
    }
}
