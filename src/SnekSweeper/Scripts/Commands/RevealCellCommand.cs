using System.Threading.Tasks;
using SnekSweeper.CellSystem;

namespace SnekSweeper.Commands;

public class RevealCellCommand(Cell cell) : ICommand
{
    public string Name => $"reveal cell at {cell.GridIndex}";

    public Task ExecuteAsync()
    {
        return cell.Reveal();
    }

    public Task UndoAsync()
    {
        return cell.PutOnCover();
    }
}