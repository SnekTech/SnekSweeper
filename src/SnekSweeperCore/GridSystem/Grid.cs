using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;

namespace SnekSweeperCore.GridSystem;

public class Grid(IHumbleGrid humbleGrid, Cell[,] cells, GridEventBus gridEventBus)
{
    readonly TransitioningCellsSet _transitioningCellsSet = new();
    internal GridSize Size { get; } = cells.Size;

    bool IsTransitioningAt(GridIndex index) => _transitioningCellsSet.Contains(index);

    public bool IsValidIndex(GridIndex gridIndex) => gridIndex.IsWithin(Size);

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

        int GetNeighborBombCount(Cell cell)
            => cell.GetNeighbors(Size, cells.At).Count(neighbor => neighbor.HasBomb);

        Task InitAllCellsAsync()
        {
            var initCellTasks = Cells
                .Select(cell => (cell, neighborBombCount: GetNeighborBombCount(cell)))
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

        var neighbors = cell.GetNeighbors(Size, cells.At).ToList();
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
        var cell = cells.At(gridIndex);
        var visited = cellsToReveal.Contains(cell);
        if (visited || !cell.IsCovered)
            return;

        cellsToReveal.Add(cell);

        if (cell.HasBomb || cell.NeighborBombCount > 0)
            return;

        foreach (var neighborIndex in cell.GridIndex.GetNeighborIndicesWithin(Size))
        {
            FindCellsToReveal(neighborIndex, cellsToReveal);
        }
    }
}