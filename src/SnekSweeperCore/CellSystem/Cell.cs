using SnekSweeperCore.CellSystem.StateMachine;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

public class Cell
{
    public Cell(IHumbleCell humbleCell, GridIndex gridIndex, CellLogic logic)
    {
        HumbleCell = humbleCell;
        GridIndex = gridIndex;
        _logic = logic;
    }

    public IHumbleCell HumbleCell { get; }

    public GridIndex GridIndex { get; }
    public bool HasBomb { get; private set; }

    readonly CellLogic _logic;

    public bool IsCovered => _logic.IsCovered;
    public bool IsRevealed => _logic.IsRevealed;
    public bool IsFlagged => _logic.IsFlagged;
    public bool IsWrongFlagged => IsFlagged && !HasBomb;
    public bool IsRevealedBomb => IsRevealed && HasBomb;

    public Task InitAsync(CellInitData cellInitData, CancellationToken ct = default)
    {
        HumbleCell.OnInit(cellInitData);
        HasBomb = cellInitData.HasBomb;
        _logic.Init(this);
        return Task.CompletedTask;
    }

    public Task RevealAsync(CancellationToken ct = default)
    {
        _logic.Reveal();
        return Task.CompletedTask;
    }

    public Task PutOnCoverAsync(CancellationToken ct = default)
    {
        _logic.PutOnCover();
        return Task.CompletedTask;
    }

    public Task SwitchFlagAsync(CancellationToken ct = default)
    {
        _logic.SwitchFlag();
        return Task.CompletedTask;
    }

    public Task MarkErrorAsync(CancellationToken ct = default)
    {
        _logic.MarkError();
        return Task.CompletedTask;
    }
}
