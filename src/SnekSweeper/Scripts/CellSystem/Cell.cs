using SnekSweeper.CellSystem.Components;
using SnekSweeper.CellSystem.StateMachine;

namespace SnekSweeper.CellSystem;

public class Cell
{
    private readonly IHumbleCell _humbleCell;
    private readonly CellStateMachine _stateMachine;

    public Cell(IHumbleCell humbleCell, (int i, int j) gridIndex, bool hasBomb = false)
    {
        _humbleCell = humbleCell;
        _stateMachine = new CellStateMachine(this);

        GridIndex = gridIndex;
        HasBomb = hasBomb;

        _humbleCell.SetPosition(gridIndex);
    }

    public ICover Cover => _humbleCell.Cover;
    public IFlag Flag => _humbleCell.Flag;

    public (int i, int j) GridIndex { get; }
    public bool HasBomb { get; set; }

    public bool IsCovered => _stateMachine.CurrentState == _stateMachine.CachedCoveredState;
    public bool IsRevealed => _stateMachine.CurrentState == _stateMachine.CachedRevealedState;
    public bool IsFlagged => _stateMachine.CurrentState == _stateMachine.CachedFlaggedState;

    public int NeighborBombCount { get; private set; }

    public void Init(int neighborBombCount)
    {
        NeighborBombCount = neighborBombCount;
        _humbleCell.SetContent(HasBomb, NeighborBombCount);
        _stateMachine.SetInitState(_stateMachine.CachedCoveredState);
    }

    public void Reveal()
    {
        _stateMachine.Reveal();
    }

    public void SwitchFlag()
    {
        _stateMachine.SwitchFlag();
    }
}