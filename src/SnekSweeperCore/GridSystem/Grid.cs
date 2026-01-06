using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;

namespace SnekSweeperCore.GridSystem;

public class Grid(IHumbleGrid humbleGrid, Cell[,] cells, GridEventBus gridEventBus)
{
    readonly TransitioningCellsSet _transitioningCellsSet = new();
    internal GridSize Size { get; } = cells.Size;

    bool IsTransitioningAt(GridIndex index) => _transitioningCellsSet.Contains(index);

    public bool IsValidIndex(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        var (rows, columns) = Size;
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    public IEnumerable<Cell> Cells => cells.Elements;

    int BombCount => Cells.Count(cell => cell.HasBomb);
    int FlagCount => Cells.Count(cell => cell.IsFlagged);

    public async Task InitCellsAsync(GridIndex firstClickGridIndex, bool[,] bombs,
        CancellationToken cancellationToken = default)
    {
        foreach (var cell in cells)
        {
            cell.HasBomb = bombs.At(cell.GridIndex);
        }

        // must init individual cells after bombs planted
        await InitAllCellsAsync();

        humbleGrid.Referee.MarkRunStartInfo(DateTime.Now, firstClickGridIndex);
        humbleGrid.TriggerInitEffects();
        gridEventBus.EmitBombCountChanged(BombCount);
        return;

        Task InitAllCellsAsync()
        {
            var initCellTasks = Cells.Select(cell =>
                    (cell, neighborBombCount: GetNeighborsOf(cell).Count(neighbor => neighbor.HasBomb)))
                .Select(t => t.cell.InitAsync(t.neighborBombCount, cancellationToken));
            return Task.WhenAll(initCellTasks);
        }
    }

    public async Task OnPrimaryReleasedAt(GridIndex gridIndex, CancellationToken cancellationToken = default)
    {
        // todo: DDD for player input
        if (IsTransitioningAt(gridIndex)) return;

        await RevealAt(gridIndex, cancellationToken);
    }

    public async Task OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        if (IsTransitioningAt(gridIndex)) return;

        await RevealAround(gridIndex);
    }

    public async Task OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        if (IsTransitioningAt(gridIndex)) return;

        await cells.At(gridIndex).SwitchFlag();
        gridEventBus.EmitFlagCountChanged(FlagCount);
    }

    IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        // todo: move to Cell extension
        var (i, j) = cell.GridIndex;
        return GridIndex.NeighborOffsets
            .Select(offset => new GridIndex(i + offset.offsetI, j + offset.offsetJ))
            .Where(IsValidIndex)
            .Select(cells.At);
    }

    async Task RevealAt(GridIndex gridIndex, CancellationToken cancellationToken = default)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        await RevealCells(cellsToReveal, cancellationToken);
    }

    async Task RevealAround(GridIndex gridIndex)
    {
        var cell = cells.At(gridIndex);
        var canRevealAround = cell is { IsRevealed: true, HasBomb: false };
        if (!canRevealAround)
            return;

        var neighbors = GetNeighborsOf(cell).ToList();
        var flaggedNeighborCount = neighbors.Count(neighbor => neighbor.IsFlagged);
        if (flaggedNeighborCount != cell.NeighborBombCount)
            return;

        var cellsToReveal = new HashSet<Cell>();
        foreach (var neighbor in neighbors)
        {
            FindCellsToReveal(neighbor.GridIndex, cellsToReveal);
        }

        await RevealCells(cellsToReveal);
    }

    async Task RevealCells(HashSet<Cell> cellsToReveal, CancellationToken cancellationToken = default)
    {
        if (cellsToReveal.Count == 0)
            return;

        _transitioningCellsSet.AddRange(cellsToReveal);
        await ExecuteRevealBatchCommandAsync(cellsToReveal, cancellationToken);
        _transitioningCellsSet.RemoveRange(cellsToReveal);

        var bombCellsRevealed = cellsToReveal.Where(cell => cell.HasBomb).ToList();
        humbleGrid.Referee.JudgeGame(this, bombCellsRevealed);

        gridEventBus.EmitBatchRevealed();
    }

    async Task ExecuteRevealBatchCommandAsync(ICollection<Cell> cellsToReveal,
        CancellationToken cancellationToken = default)
    {
        var commands = cellsToReveal.Select(cell => new RevealCellCommand(cell));
        await humbleGrid.GridCommandInvoker.ExecuteCommandAsync(new CompoundCommand(commands), cancellationToken);
    }

    void FindCellsToReveal(GridIndex gridIndex, ICollection<Cell> cellsToReveal)
    {
        if (!IsValidIndex(gridIndex))
            return;

        var cell = cells.At(gridIndex);
        var visited = cellsToReveal.Contains(cell);
        if (visited || !cell.IsCovered)
            return;

        cellsToReveal.Add(cell);

        if (cell.HasBomb || cell.NeighborBombCount > 0)
            return;

        foreach (var neighbor in GetNeighborsOf(cell))
        {
            FindCellsToReveal(neighbor.GridIndex, cellsToReveal);
        }
    }
}