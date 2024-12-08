using System;
using System.Linq;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.CellSystem.StateMachine;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    public event Action<(int i, int j)>? PrimaryReleased;
    public event Action<(int i, int j)>? PrimaryDoubleClicked;
    public event Action<(int i, int j)>? SecondaryReleased;
    
    private readonly IHumbleCell _humbleCell;
    private readonly CellStateMachine _stateMachine;

    public Cell(IHumbleCell humbleCell, (int i, int j) gridIndex, bool hasBomb = false)
    {
        _humbleCell = humbleCell;
        _stateMachine = new CellStateMachine(this);

        GridIndex = gridIndex;
        HasBomb = hasBomb;

        _humbleCell.SetPosition(gridIndex);

        _humbleCell.PrimaryReleased += OnPrimaryReleased;
        _humbleCell.PrimaryDoubleClicked += OnPrimaryDoubleClicked;
        _humbleCell.SecondaryReleased += OnSecondaryReleased;
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

    public void OnDispose()
    {
        _humbleCell.PrimaryReleased -= OnPrimaryReleased;
        _humbleCell.PrimaryDoubleClicked -= OnPrimaryDoubleClicked;
        _humbleCell.SecondaryReleased -= OnSecondaryReleased;
    }

    private void OnPrimaryReleased()
    {
        PrimaryReleased?.Invoke(GridIndex);
    }

    private void OnPrimaryDoubleClicked()
    {
        PrimaryDoubleClicked?.Invoke(GridIndex);
    }

    private void OnSecondaryReleased()
    {
        SecondaryReleased?.Invoke(GridIndex);
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