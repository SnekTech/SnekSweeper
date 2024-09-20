using System;
using System.Collections.Generic;
using System.Linq;
using SnekSweeper.CellSystem;
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

    private readonly Cell[,] _cells;
    private readonly IHumbleGrid _humbleGrid;
    private readonly IGridDifficulty _gridDifficulty;
    private bool _hasInitialized;

    public Grid(IHumbleGrid humbleGrid, IGridDifficulty gridDifficulty)
    {
        _humbleGrid = humbleGrid;
        _gridDifficulty = gridDifficulty;

        var (rows, columns) = gridDifficulty.Size;
        _cells = new Cell[rows, columns];

        InstantiateEmptyCells();
        
    }

    private void InstantiateEmptyCells()
    {
        var humbleCells = _humbleGrid.InstantiateHumbleCells(_cells.Length);
        foreach (var (i, j) in _cells.Indices())
        {
            var humbleCell = humbleCells[i * _cells.Size().columns + j];
            var cell = new Cell(humbleCell, this, (i, j));
            _cells[i, j] = cell;
        }
    }

    private void InitCells(BombMatrix bombMatrix)
    {
        foreach (var (i, j) in _cells.Indices())
        {
            var cell = _cells[i, j];
            cell.HasBomb = bombMatrix[i, j];
        }

        // must init individual cells after all cells been set if bomb
        foreach (var cell in _cells)
        {
            cell.Init();
        }
        
        HistoryManager.CurrentRecordStartAt = DateTime.Now;
    }

    public void OnDispose()
    {
        foreach (var cell in _cells)
        {
            cell.OnDispose();
        }
    }

    public void OnCellPrimaryReleasedAt((int i, int j) gridIndex)
    {
        if (!_hasInitialized)
        {
            var bombMatrix = new BombMatrix(_gridDifficulty);
            bombMatrix.ClearBombAt(gridIndex);
            InitCells(bombMatrix);
            _hasInitialized = true;
        }

        RevealAt(gridIndex);
    }

    public void OnCellPrimaryDoubleClickedAt((int i, int j) gridIndex)
    {
        RevealAround(gridIndex);
    }

    public void OnCellSecondaryReleasedAt((int i, int j) gridIndex)
    {
        this[gridIndex].SwitchFlag();
    }

    private Cell this[(int i, int j) gridIndex]
    {
        get
        {
            var (i, j) = gridIndex;
            return _cells[i, j];
        }
    }

    private bool IsValidIndex((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        var (rows, columns) = _cells.Size();
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    public IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in NeighborOffsets)
        {
            var neighborIndex = (i + offsetI, j + offsetJ);

            if (IsValidIndex(neighborIndex))
            {
                yield return this[neighborIndex];
            }
        }
    }

    private void RevealAt((int i, int j) gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        var bombCellsRevealed = new List<Cell>();
        foreach (var cell in cellsToReveal)
        {
            cell.Reveal();
            if (cell.HasBomb)
            {
                bombCellsRevealed.Add(cell);
            }
        }

        if (bombCellsRevealed.Count > 0)
        {
            BombRevealed?.Invoke(bombCellsRevealed);
        }

        BatchRevealed?.Invoke();
    }

    private void RevealAround((int i, int j) gridIndex)
    {
        var cell = this[gridIndex];
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

        foreach (var c in cellsToReveal)
        {
            c.Reveal();
        }

        BatchRevealed?.Invoke();
    }

    private void FindCellsToReveal((int i, int j) gridIndex, ICollection<Cell> cellsToReveal)
    {
        if (!IsValidIndex(gridIndex))
            return;

        var cell = this[gridIndex];
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
}