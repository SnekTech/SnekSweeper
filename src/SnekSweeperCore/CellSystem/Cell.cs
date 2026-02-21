using SnekSweeperCore.CellSystem.StateMachine;
using SnekSweeperCore.CellSystem.StateMachine.States;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

public class Cell
{
    public Cell(IHumbleCell humbleCell, GridIndex gridIndex)
    {
        HumbleCell = humbleCell;
        GridIndex = gridIndex;
        _stateMachine = new CellStateMachine(this);
    }

    public IHumbleCell HumbleCell { get; }

    public GridIndex GridIndex { get; }
    public bool HasBomb { get; set; }

    readonly CellStateMachine _stateMachine;


    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();
    public bool IsWrongFlagged => IsFlagged && !HasBomb;
    public bool IsRevealedBomb => IsRevealed && HasBomb;

    public async Task InitAsync(CellInitData cellInitData, CancellationToken ct = default)
    {
        HumbleCell.OnInit(cellInitData);
        HasBomb = cellInitData.HasBomb;
        await _stateMachine.SetInitStateAsync<CoveredState>(ct);
    }

    public Task RevealAsync(CancellationToken ct = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.RevealCover, ct);

    public Task PutOnCoverAsync(CancellationToken ct = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.PutOnCover, ct);

    public Task SwitchFlagAsync(CancellationToken ct = default) =>
        _stateMachine.HandleCellRequestAsync(IsFlagged ? CellRequest.PutDownFlag : CellRequest.RaiseFlag, ct);

    public Task MarkErrorAsync(CancellationToken ct = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.MarkError, ct);
}