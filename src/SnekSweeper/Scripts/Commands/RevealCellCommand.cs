using SnekSweeper.CellSystem;

namespace SnekSweeper.Commands;

public class RevealCellCommand : ICommand
{
    private readonly Cell _cell;

    public RevealCellCommand(Cell cell)
    {
        _cell = cell;
    }

    public string Name => $"reveal cell at {_cell.GridIndex}";

    public void Execute()
    {
        _cell.Reveal();
    }

    public void Undo()
    {
        _cell.PutOnCover();
    }
}