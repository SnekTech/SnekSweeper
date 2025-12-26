using SnekSweeperCore.CellSystem;

namespace SnekSweeper.Commands;

public class RevealCellCommand(Cell cell) : ICommand
{
    public string Name => $"reveal cell at {cell.GridIndex}";

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return cell.RevealAsync(cancellationToken);
    }

    public Task UndoAsync()
    {
        return cell.PutOnCover();
    }
}