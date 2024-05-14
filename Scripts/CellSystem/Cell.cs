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

    // todo: add cell covered state using FSM
    public bool IsCovered => true;
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

    public void Init()
    {
        _humbleCell.SetContent(this);
        _humbleCell.SetPosition(GridIndex);
        _humbleCell.PrimaryReleased += OnPrimaryReleased;
    }

    private void OnPrimaryReleased()
    {
        _parent.RevealAt(GridIndex);
    }

    public void Reveal()
    {
        _humbleCell.Reveal();
    }
}