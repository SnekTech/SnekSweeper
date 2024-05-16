using System.Linq;
using SnekSweeper.CellSystem.StateMachine;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    private readonly IHumbleCell _humbleCell;
    private readonly Grid _parent;
    private readonly CellStateMachine _stateMachine;

    public Cell(IHumbleCell humbleCell, Grid parent, (int i, int j) gridIndex, bool hasBomb)
    {
        _humbleCell = humbleCell;
        _parent = parent;
        _stateMachine = new CellStateMachine(this);

        GridIndex = gridIndex;
        HasBomb = hasBomb;
    }

    public (int i, int j) GridIndex { get; }
    public bool HasBomb { get; }

    public bool IsCovered => _stateMachine.CurrentStateValue == CellStateValue.Covered;

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
        
        // no place to unsubscribe, for now
        _humbleCell.PrimaryReleased += OnPrimaryReleased;
        
        _stateMachine.SetInitState(_stateMachine.CachedCoveredState);
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