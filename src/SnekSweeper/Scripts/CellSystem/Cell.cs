using SnekSweeper.CellSystem.Components;
using SnekSweeper.CellSystem.StateMachine;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    private readonly CellStateMachine _stateMachine;

    public Cell(IHumbleCell humbleCell, GridIndex gridIndex, bool hasBomb = false)
    {
        HumbleCell = humbleCell;
        _stateMachine = new CellStateMachine(humbleCell);

        GridIndex = gridIndex;
        HasBomb = hasBomb;

        HumbleCell.SetPosition(gridIndex);
    }

    public IHumbleCell HumbleCell { get; }

    public GridIndex GridIndex { get; }
    public bool HasBomb { get; set; }
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