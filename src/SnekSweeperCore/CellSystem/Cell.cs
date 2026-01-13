using SnekSweeperCore.CellSystem.StateMachine;
using SnekSweeperCore.CellSystem.StateMachine.States;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

public class Cell(IHumbleCell humbleCell, GridIndex gridIndex, bool hasBomb = false)
{
    readonly CellStateMachine _stateMachine = new(humbleCell);

    public IHumbleCell HumbleCell { get; } = humbleCell;

    public GridIndex GridIndex { get; } = gridIndex;
    public bool HasBomb { get; set; } = hasBomb;

    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();

    public async Task InitAsync(int neighborBombCount, CancellationToken cancellationToken = default)
    {
        HumbleCell.SetContent(HasBomb, neighborBombCount);
        await _stateMachine.SetInitStateAsync<CoveredState>(cancellationToken);
    }

    public Task RevealAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.RevealCover, cancellationToken);

    public Task PutOnCoverAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.PutOnCover, cancellationToken);

    public Task SwitchFlagAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.HandleCellRequestAsync(IsFlagged ? CellRequest.PutDownFlag : CellRequest.RaiseFlag,
            cancellationToken);
}