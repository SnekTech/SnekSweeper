using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.CellSystem;

public class Cell(IHumbleCell humbleCell, GridIndex gridIndex, bool hasBomb = false)
{
    readonly CellLogic _logic = new(humbleCell);

    public IHumbleCell HumbleCell { get; } = humbleCell;

    public GridIndex GridIndex { get; } = gridIndex;
    public bool HasBomb { get; set; } = hasBomb;

    public bool IsCovered => _logic.IsCovered;
    public bool IsRevealed => _logic.IsRevealed;
    public bool IsFlagged => _logic.IsFlagged;

    public async Task InitAsync(int neighborBombCount, CancellationToken cancellationToken = default)
    {
        HumbleCell.SetContent(HasBomb, neighborBombCount);
        await _logic.InitAsync(cancellationToken);
    }

    public Task RevealAsync(CancellationToken cancellationToken = default) => _logic.RevealAsync(cancellationToken);

    public Task PutOnCoverAsync(CancellationToken cancellationToken = default) =>
        _logic.PutOnCoverAsync(cancellationToken);

    public Task SwitchFlagAsync(CancellationToken cancellationToken = default) =>
        _logic.SwitchFlagAsync(cancellationToken);
}