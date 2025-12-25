using SnekSweeper.CellSystem.StateMachine;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell(IHumbleCell humbleCell, GridIndex gridIndex, bool hasBomb = false)
{
    readonly CellStateMachine _stateMachine = new(humbleCell);

    public IHumbleCell HumbleCell { get; } = humbleCell;

    public GridIndex GridIndex { get; } = gridIndex;
    public bool HasBomb { get; set; } = hasBomb;
    public int NeighborBombCount { get; private set; }

    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();

    public async Task InitAsync(int neighborBombCount)
    {
        NeighborBombCount = neighborBombCount;
        HumbleCell.SetContent(HasBomb, NeighborBombCount);
        await _stateMachine.SetInitStateAsync<CoveredState>();
    }

    public Task Reveal() => _stateMachine.HandleCellRequestAsync(CellRequest.RevealCover);

    public Task PutOnCover() => _stateMachine.HandleCellRequestAsync(CellRequest.PutOnCover);

    public Task SwitchFlag() =>
        _stateMachine.HandleCellRequestAsync(IsFlagged ? CellRequest.PutDownFlag : CellRequest.RaiseFlag);
}