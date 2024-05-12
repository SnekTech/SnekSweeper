using System.Linq;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    private readonly IHumbleCell _humbleCell;
    private readonly Grid _parent;

    public Cell(IHumbleCell humbleCell, Grid parent, (int i, int j) gridIndex, bool hasBomb)
    {
        _humbleCell = humbleCell;
        _parent = parent;
        GridIndex = gridIndex;
        HasBomb = hasBomb;
    }

    public void Init()
    {
        _humbleCell.Init(this);
    }

    public (int i, int j) GridIndex { get; }
    public bool HasBomb { get; }

    public int NeighborBombCount
    {
        get
        {
            if (HasBomb)
                return -1;

            return _parent.GetNeighborsOf(this).Count(neighbor => neighbor.HasBomb);
        }
    }
}