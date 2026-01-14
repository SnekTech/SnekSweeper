using SnekSweeperCore.CellSystem.StateMachine;
using SnekSweeperCore.CellSystem.StateMachine.States;

namespace SnekSweeperCore.CellSystem;

public class CellLogic(IHumbleCell humbleCell)
{
    readonly CellStateMachine _stateMachine = new(humbleCell);

    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
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