using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.Commands;

public class RevealCellCommand(GridIndex index) : ICommand
{
    public string Name => $"reveal cell at {index}";

    public void Execute(Grid grid) => grid.ApplyCommand(index, new CellCommand.RevealCover());

    public void Undo(Grid grid) => grid.ApplyCommand(index, new CellCommand.PutOnCover());
}
