using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

public class TransitioningCellsSet
{
    private readonly HashSet<GridIndex> _transitioningCellIndexes = [];

    public bool Contains(GridIndex gridIndex) => _transitioningCellIndexes.Contains(gridIndex);

    public void AddRange(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells)
        {
            _transitioningCellIndexes.Add(cell.GridIndex);
        }
    }

    public void RemoveRange(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells)
        {
            _transitioningCellIndexes.Remove(cell.GridIndex);
        }
    }
}