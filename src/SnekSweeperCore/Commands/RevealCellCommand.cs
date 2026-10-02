using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class RevealCellCommand(GridIndex index) : ICommand
{
    public string Name => $"reveal cell at {index}";

    public GridOutcome Execute(Grid grid)
    {
        var cellOutcome = grid.ApplyCommand(index, new CellCommand.RevealCover());
        return cellOutcome is null
            ? new GridOutcome.NothingHappens()
            : new GridOutcome.BatchRevealed([cellOutcome]);
    }

    public GridOutcome Undo(Grid grid)
    {
        var cellOutcome = grid.ApplyCommand(index, new CellCommand.PutOnCover());
        return cellOutcome is null
            ? new GridOutcome.NothingHappens()
            : new GridOutcome.BatchCovered([cellOutcome]);
    }
}