using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;

namespace SnekSweeperCore.GridSystem;

public class Grid(IHumbleGrid humbleGrid, Cell[,] cells, GridEventBus gridEventBus)
{
    bool _isAnyCellProcessing;
    public GridSize Size { get; } = cells.Size;

    public IEnumerable<Cell> Cells => cells.Elements;

    int BombCount => Cells.Count(cell => cell.HasBomb);
    int FlagCount => Cells.Count(cell => cell.IsFlagged);

    public async Task InitCellsAsync(bool[,] bombs, CancellationToken ct = default)
    {
        foreach (var cell in cells)
        {
            cell.HasBomb = bombs.At(cell.GridIndex);
        }

        // must init individual cells *after* bombs planted
        _isAnyCellProcessing = true;
        await InitAllCellsAsync();
        _isAnyCellProcessing = false;

        gridEventBus.EmitBombCountChanged(BombCount);
        return;

        Task InitAllCellsAsync()
        {
            var initCellTasks = Cells
                .Select(cell => cell.InitAsync(new CellInitData(cell.HasBomb, GetNeighborBombCount(cell)), ct));
            return Task.WhenAll(initCellTasks);
        }
    }

    public async Task<GridInputProcessResult> HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (!gridInput.Index.IsWithin(Size) || _isAnyCellProcessing)
            return NothingHappens.Instance;

        _isAnyCellProcessing = true;
        var processResult = await (gridInput switch
        {
            PrimaryReleased => RevealAtAsync(gridInput.Index, ct),
            PrimaryDoubleClicked => RevealAroundAsync(gridInput.Index, ct),
            SecondaryReleased => SwitchFlagAtAsync(gridInput.Index, ct),
            _ => throw new SwitchExpressionException(),
        });
        _isAnyCellProcessing = false;

        return processResult;
    }

    async Task<GridInputProcessResult> RevealAtAsync(GridIndex gridIndex, CancellationToken ct = default)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        return await RevealCells(cellsToReveal, ct);
    }

    async Task<GridInputProcessResult> RevealAroundAsync(GridIndex gridIndex, CancellationToken ct = default)
    {
        var cell = cells.At(gridIndex);
        if (!CanRevealAround())
            return NothingHappens.Instance;

        var cellsToReveal = new HashSet<Cell>();
        foreach (var neighbor in GetNeighbors(cell))
        {
            FindCellsToReveal(neighbor.GridIndex, cellsToReveal);
            cellsToReveal.Add(neighbor);
        }

        return await RevealCells(cellsToReveal, ct);

        bool CanRevealAround()
        {
            var cellIsRevealed = cell is { IsRevealed: true, HasBomb: false };
            var flagCountMatchesBomb = GetNeighborFlagCount(cell) == GetNeighborBombCount(cell);
            return cellIsRevealed && flagCountMatchesBomb;
        }
    }

    IEnumerable<Cell> GetNeighbors(Cell cell) => cell.GridIndex.GetNeighborIndicesWithin(Size).Select(cells.At);
    int GetNeighborBombCount(Cell cell) => GetNeighbors(cell).Count(neighbor => neighbor.HasBomb);
    int GetNeighborFlagCount(Cell cell) => GetNeighbors(cell).Count(neighbor => neighbor.IsFlagged);

    async Task<GridInputProcessResult> SwitchFlagAtAsync(GridIndex gridIndex, CancellationToken ct = default)
    {
        await cells.At(gridIndex).SwitchFlagAsync(ct);
        gridEventBus.EmitFlagCountChanged(FlagCount);
        return FlagSwitched.Instance;
    }

    async Task<GridInputProcessResult> RevealCells(HashSet<Cell> cellsToReveal, CancellationToken ct = default)
    {
        if (cellsToReveal.Count == 0)
            return NothingHappens.Instance;

        await ExecuteRevealBatchCommandAsync();

        gridEventBus.EmitBatchRevealed();

        return new BatchRevealed(this, cellsToReveal.ToList());

        Task ExecuteRevealBatchCommandAsync()
        {
            var commands = cellsToReveal.Select(cell => new RevealCellCommand(cell));
            return humbleGrid.GridCommandInvoker.ExecuteCommandAsync(new CompoundCommand(commands), ct);
        }
    }

    void FindCellsToReveal(GridIndex gridIndex, ICollection<Cell> cellsToReveal)
    {
        var cell = cells.At(gridIndex);
        var visited = cellsToReveal.Contains(cell);
        if (visited || !cell.IsCovered)
            return;

        cellsToReveal.Add(cell);

        if (cell.HasBomb || GetNeighborBombCount(cell) > 0)
            return;

        foreach (var neighborIndex in cell.GridIndex.GetNeighborIndicesWithin(Size))
        {
            FindCellsToReveal(neighborIndex, cellsToReveal);
        }
    }
}