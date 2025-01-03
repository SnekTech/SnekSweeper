using System;
using System.Collections.Generic;
using System.Linq;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameHistory;

namespace SnekSweeper.GridSystem;

public class Grid
{
    public event Action<List<Cell>>? BombRevealed;
    public event Action? BatchRevealed;

    private static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
    {
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1),
    };

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

    public bool IsValidIndex((int i, int j) gridIndex)
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
            var cell = new Cell(humbleCell, (i, j));
            _cells[i, j] = cell;
        }
    }

    private void InitCells((int i, int j) firstClickGridIndex)
    {
        _bombMatrix.ClearBombAt(firstClickGridIndex);
        foreach (var (i, j) in _cells.Indices())
        {
            _cells[i, j].HasBomb = _bombMatrix[i, j];
        }

        // must init individual cells after bombs planted
        foreach (var cell in _cells)
        {
            var neighborBombCount = GetNeighborsOf(cell).Count(neighbor => neighbor.HasBomb);
            cell.Init(neighborBombCount);
        }

        _hasCellInitialized = true;

        _eventBus.EmitBombCountChanged(BombCount);
        HistoryManager.CurrentRecordStartAt = DateTime.Now;
    }

    public void OnPrimaryReleasedAt((int i, int j) gridIndex)
    {
        if (!_hasCellInitialized)
        {
            InitCells(gridIndex);
        }

        RevealAt(gridIndex);
    }

    public void OnPrimaryDoubleClickedAt((int i, int j) gridIndex)
    {
        RevealAround(gridIndex);
    }

    public void OnSecondaryReleasedAt((int i, int j) gridIndex)
    {
        GetCellAt(gridIndex).SwitchFlag();
        _eventBus.EmitFlagCountChanged(FlagCount);
    }

    private Cell GetCellAt((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        return _cells[i, j];
    }

    private IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in NeighborOffsets)
        {
            var neighborIndex = (i + offsetI, j + offsetJ);
            if (IsValidIndex(neighborIndex))
            {
                yield return GetCellAt(neighborIndex);
            }
        }
    }

    private void RevealAt((int i, int j) gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        RevealCells(cellsToReveal);
    }

    private void RevealAround((int i, int j) gridIndex)
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

        RevealCells(cellsToReveal);
    }

    private void RevealCells(ICollection<Cell> cells)
    {
        if (cells.Count == 0)
            return;

        ExecuteRevealBatchCommand(cells);

        var bombCellsRevealed = cells.Where(cell => cell.HasBomb).ToList();
        if (bombCellsRevealed.Count > 0)
        {
            BombRevealed?.Invoke(bombCellsRevealed);
        }

        BatchRevealed?.Invoke();
    }

    private void ExecuteRevealBatchCommand(ICollection<Cell> cellsToReveal)
    {
        var commands = cellsToReveal.Select(cell => new RevealCellCommand(cell));
        _commandInvoker.ExecuteCommand(new CompoundCommand(commands));
    }

    private void FindCellsToReveal((int i, int j) gridIndex, ICollection<Cell> cellsToReveal)
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