using System.Collections.Generic;
using System.Linq;
using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public class Grid
{
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

    private Cell[,] _cells;
    private readonly IHumbleGrid _humbleGrid;

    public Grid(IHumbleGrid humbleGrid)
    {
        _humbleGrid = humbleGrid;
    }

    public void InitCells(BombMatrix bombMatrix)
    {
        var (rows, columns) = bombMatrix.Size;
        _cells = new Cell[rows, columns];

        var humbleCells = _humbleGrid.InstantiateHumbleCells(_cells.Length);
        foreach (var (i, j) in _cells.Indices())
        {
            var humbleCell = humbleCells[i * columns + j];
            var hasBomb = bombMatrix[i, j];

            var cell = new Cell(humbleCell, this, (i, j), hasBomb);
            _cells[i, j] = cell;
        }

        // must init individual cells after all cells been created,
        // otherwise neighbor might be null
        foreach (var cell in _cells.Values())
        {
            cell.Init();
        }
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

    public void RevealAt((int i, int j) gridIndex)
    {
        var cellsToReveal = new HashSet<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);

        foreach (var cell in cellsToReveal)
        {
            cell.Reveal();
        }
    }

    public void RevealAround((int i, int j) gridIndex)
    {
        var cell = this[gridIndex];
        var canRevealAround = cell.IsRevealed && !cell.HasBomb;
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
}