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
    public int NeighborBombCount { get; private set; }

    public bool IsCovered => _stateMachine.IsAtState<CoveredState>();
    public bool IsRevealed => _stateMachine.IsAtState<RevealedState>();
    public bool IsFlagged => _stateMachine.IsAtState<FlaggedState>();

    public IEnumerable<Cell> GetNeighbors(GridSize size, Func<GridIndex, Cell> cellGetter)
        => GridIndex.GetNeighborIndicesWithin(size).Select(cellGetter);

    public async Task InitAsync(int neighborBombCount, CancellationToken cancellationToken = default)
    {
        NeighborBombCount = neighborBombCount;
        HumbleCell.SetContent(HasBomb, NeighborBombCount);
        await _stateMachine.SetInitStateAsync<CoveredState>(cancellationToken);
    }

    public Task RevealAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.HandleCellRequestAsync(CellRequest.RevealCover, cancellationToken);

    public Task PutOnCoverAsync(CancellationToken cancellationToken = default) => _stateMachine.HandleCellRequestAsync(CellRequest.PutOnCover, cancellationToken);

    public Task SwitchFlagAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.HandleCellRequestAsync(IsFlagged ? CellRequest.PutDownFlag : CellRequest.RaiseFlag, cancellationToken);
}