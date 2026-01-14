using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

public class Cell(IHumbleCell humbleCell, GridIndex gridIndex)
{
    readonly CellLogic _logic = new(humbleCell);

    public IHumbleCell HumbleCell { get; } = humbleCell;

    public GridIndex GridIndex { get; } = gridIndex;
    public bool HasBomb { get; set; }

    public bool IsCovered => _logic.IsCovered;
    public bool IsRevealed => _logic.IsRevealed;
    public bool IsFlagged => _logic.IsFlagged;

    public async Task InitAsync(CellInitData cellInitData, CancellationToken cancellationToken = default)
    {
        HumbleCell.OnInit(cellInitData);
        HasBomb = cellInitData.HasBomb;
        await _logic.InitAsync(cancellationToken);
    }

    public Task RevealAsync(CancellationToken cancellationToken = default) => _logic.RevealAsync(cancellationToken);

    public Task PutOnCoverAsync(CancellationToken cancellationToken = default) =>
        _logic.PutOnCoverAsync(cancellationToken);

    public Task SwitchFlagAsync(CancellationToken cancellationToken = default) =>
        _logic.SwitchFlagAsync(cancellationToken);
}