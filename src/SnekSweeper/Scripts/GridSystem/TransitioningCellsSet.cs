using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public class TransitioningCellsSet
{
    private readonly HashSet<GridIndex> _transitioningCellIndexes = [];

    public bool IsEmpty => _transitioningCellIndexes.Count == 0;

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