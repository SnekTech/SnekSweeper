using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.Constants;
using SnekSweeper.GameHistory;

namespace SnekSweeper.GridSystem;

public class Grid
{
    public event Action<List<Cell>>? BombRevealed;
    public event Action? BatchRevealed;

    private readonly BombMatrix _bombMatrix;
    private readonly Cell[,] _cells;
    private readonly IHumbleGrid _humbleGrid;
    private bool _hasCellInitialized;
    private readonly GridEventBus _eventBus = EventBusOwner.GridEventBus;
    private readonly CommandInvoker _commandInvoker;

    public Grid(IHumbleGrid humbleGrid, BombMatrix bombMatrix)
    {
        _humbleGrid = humbleGrid;
        _commandInvoker = humbleGrid.GridCommandInvoker;
        _bombMatrix = bombMatrix;

        var (rows, columns) = bombMatrix.Size;
        _cells = new Cell[rows, columns];

        InstantiateHumbleCells();
    }

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

    public bool IsValidIndex(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        var (rows, columns) = _cells.Size();
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    private int BombCount => _cells.Cast<Cell>().Count(cell => cell.HasBomb);
    private int FlagCount => _cells.Cast<Cell>().Count(cell => cell.IsFlagged);

    private void InstantiateHumbleCells()
    {
        var humbleCells = _humbleGrid.InstantiateHumbleCells(_cells.Length);
        foreach (var (i, j) in _cells.Indices())
        {
            var humbleCell = humbleCells[i * _cells.Size().columns + j];
            var cell = new Cell(humbleCell, new GridIndex(i, j));
            _cells[i, j] = cell;
        }
    }

    private async Task InitCellsAsync(GridIndex firstClickGridIndex)
    {
        _bombMatrix.ClearBombAt(firstClickGridIndex);
        foreach (var (i, j) in _cells.Indices())
        {
            _cells[i, j].HasBomb = _bombMatrix[i, j];
        }

        // must init individual cells after bombs planted
        var initCellTasks = new List<Task>();
        foreach (var cell in _cells)
        {
            var neighborBombCount = GetNeighborsOf(cell).Count(neighbor => neighbor.HasBomb);
            initCellTasks.Add(cell.InitAsync(neighborBombCount));
        }
        await Task.WhenAll(initCellTasks);

        _hasCellInitialized = true;

        _eventBus.EmitBombCountChanged(BombCount);
        HistoryManager.CurrentRecordStartAt = DateTime.Now;
    }

    public async Task OnPrimaryReleasedAt(GridIndex gridIndex)
    {
        if (!_hasCellInitialized)
        {
            await InitCellsAsync(gridIndex);
        }

        await RevealAt(gridIndex);
    }

    public async Task OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        await RevealAround(gridIndex);
    }

    public async Task OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        await GetCellAt(gridIndex).SwitchFlag();
        _eventBus.EmitFlagCountChanged(FlagCount);
    }

    private Cell GetCellAt(GridIndex gridIndex)
    {
        var (i, j) = gridIndex;
        return _cells[i, j];
    }

    private IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in CoreStats.NeighborOffsets)
        {
            var neighborIndex = new GridIndex(i + offsetI, j + offsetJ);
            if (IsValidIndex(neighborIndex))
            {
                yield return GetCellAt(neighborIndex);
            }
        }
    }

    private async Task RevealAt(GridIndex gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        await RevealCells(cellsToReveal);
    }

    private async Task RevealAround(GridIndex gridIndex)
    {
        var cell = GetCellAt(gridIndex);
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

    private async Task RevealCells(ICollection<Cell> cells)
    {
        if (cells.Count == 0)
            return;

        await ExecuteRevealBatchCommandAsync(cells);

        var bombCellsRevealed = cells.Where(cell => cell.HasBomb).ToList();
        if (bombCellsRevealed.Count > 0)
        {
            BombRevealed?.Invoke(bombCellsRevealed);
        }

        BatchRevealed?.Invoke();
    }

    private async Task ExecuteRevealBatchCommandAsync(ICollection<Cell> cellsToReveal)
    {
        var commands = cellsToReveal.Select(cell => new RevealCellCommand(cell));
        await _commandInvoker.ExecuteCommandAsync(new CompoundCommand(commands));
    }

    private void FindCellsToReveal(GridIndex gridIndex, ICollection<Cell> cellsToReveal)
    {
        if (!IsValidIndex(gridIndex))
            return;

        var cell = GetCellAt(gridIndex);
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