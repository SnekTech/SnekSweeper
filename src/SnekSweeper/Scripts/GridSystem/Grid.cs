using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.Constants;
using SnekSweeper.GridSystem.LayMineStrategies;

namespace SnekSweeper.GridSystem;

public class Grid
{
    readonly Cell[,] _cells;
    readonly IHumbleGrid _humbleGrid;
    bool _hasCellInitialized;
    readonly GridEventBus _eventBus = EventBusOwner.GridEventBus;
    readonly CommandInvoker _commandInvoker;
    readonly TransitioningCellsSet _transitioningCellsSet = new();

    public Grid(IHumbleGrid humbleGrid, GridSize size, ILayMineStrategy layMineStrategy)
    {
        _humbleGrid = humbleGrid;
        _commandInvoker = humbleGrid.GridCommandInvoker;
        Size = size;
        LayMineStrategy = layMineStrategy;

        _cells = new Cell[Size.Rows, Size.Columns];

        InstantiateHumbleCells();
    }

    ILayMineStrategy LayMineStrategy { get; }
    GridSize Size { get; }

    public bool IsResolved
    {
        get
        {
            foreach (var cell in _cells)
            {
                if (cell is { HasBomb: false, IsRevealed: false })
                {
                    return false;
                }
            }

            return true;
        }
    }

    bool IsTransitioningAt(GridIndex index) => _transitioningCellsSet.Contains(index);

    public bool IsValidIndex(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        var (rows, columns) = Size;
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    public IEnumerable<Cell> Cells => _cells.Elements;
    internal bool[,] BombMatrix => _cells.MapTo(cell => cell.HasBomb);

    int BombCount => _cells.Cast<Cell>().Count(cell => cell.HasBomb);
    int FlagCount => _cells.Cast<Cell>().Count(cell => cell.IsFlagged);

    void InstantiateHumbleCells()
    {
        var humbleCells = _humbleGrid.InstantiateHumbleCells(_cells.Length);
        foreach (var (i, j) in _cells.Indices())
        {
            var humbleCell = humbleCells[i * Size.Columns + j];
            var cell = new Cell(humbleCell, new GridIndex(i, j));
            _cells[i, j] = cell;
        }
    }

    async Task InitCellsAsync(GridIndex firstClickGridIndex)
    {
        var bombs = LayMineStrategy.Generate(firstClickGridIndex);
        foreach (var index in _cells.Indices())
        {
            _cells.At(index).HasBomb = bombs.At(index);
        }

        // must init individual cells after bombs planted
        var initCellTasks =
            Cells.Select(cell =>
                    (cell, neighborBombCount: GetNeighborsOf(cell).Count(neighbor => neighbor.HasBomb)))
                .Select(t => t.cell.InitAsync(t.neighborBombCount)).ToList();

        await Task.WhenAll(initCellTasks);

        _hasCellInitialized = true;

        _eventBus.EmitInitCompleted();
        _eventBus.EmitBombCountChanged(BombCount);
    }

    public async Task OnPrimaryReleasedAt(GridIndex gridIndex)
    {
        if (!_hasCellInitialized)
        {
            await InitCellsAsync(gridIndex);
        }

        if (IsTransitioningAt(gridIndex)) return;

        await RevealAt(gridIndex);
    }

    public async Task OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        if (IsTransitioningAt(gridIndex)) return;

        await RevealAround(gridIndex);
    }

    public async Task OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        if (IsTransitioningAt(gridIndex)) return;

        await _cells.At(gridIndex).SwitchFlag();
        _eventBus.EmitFlagCountChanged(FlagCount);
    }

    IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in CoreStats.NeighborOffsets)
        {
            var neighborIndex = new GridIndex(i + offsetI, j + offsetJ);
            if (IsValidIndex(neighborIndex))
            {
                yield return _cells.At(neighborIndex);
            }
        }
    }

    async Task RevealAt(GridIndex gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        await RevealCells(cellsToReveal);
    }

    async Task RevealAround(GridIndex gridIndex)
    {
        var cell = _cells.At(gridIndex);
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

    async Task RevealCells(HashSet<Cell> cells)
    {
        if (cells.Count == 0)
            return;

        _transitioningCellsSet.AddRange(cells);
        await ExecuteRevealBatchCommandAsync(cells);
        _transitioningCellsSet.RemoveRange(cells);

        var bombCellsRevealed = cells.Where(cell => cell.HasBomb).ToList();
        if (bombCellsRevealed.Count > 0)
        {
            _eventBus.EmitBombRevealed(bombCellsRevealed);
        }

        _eventBus.EmitBatchRevealed();
    }

    async Task ExecuteRevealBatchCommandAsync(ICollection<Cell> cellsToReveal)
    {
        var commands = cellsToReveal.Select(cell => new RevealCellCommand(cell));
        await _commandInvoker.ExecuteCommandAsync(new CompoundCommand(commands));
    }

    void FindCellsToReveal(GridIndex gridIndex, ICollection<Cell> cellsToReveal)
    {
        if (!IsValidIndex(gridIndex))
            return;

        var cell = _cells.At(gridIndex);
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