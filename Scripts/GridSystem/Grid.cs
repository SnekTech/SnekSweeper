using System.Collections.Generic;
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

    public Cell this[int i, int j] => _cells[i, j];

    private bool IsValidIndex((int i, int j) gridIndex)
    {
        var (i, j) = gridIndex;
        return IsValidIndex(i, j);
    }

    private bool IsValidIndex(int i, int j)
    {
        var (rows, columns) = _cells.Size();
        return i >= 0 && i < rows && j >= 0 && j < columns;
    }

    public IEnumerable<Cell> GetNeighborsOf(Cell cell)
    {
        var (i, j) = cell.GridIndex;
        foreach (var (offsetI, offsetJ) in NeighborOffsets)
        {
            var (neighborI, neighborJ) = (i + offsetI, j + offsetJ);

            if (IsValidIndex(neighborI, neighborJ))
            {
                yield return this[neighborI, neighborJ];
            }
        }
    }

    public void RevealAt((int i, int j) gridIndex)
    {
        var cellsToReveal = new List<Cell>();
        FindCellsToReveal(gridIndex, cellsToReveal);
        
        foreach (var cell in cellsToReveal)
        {
            cell.Reveal();
        }
    }

    private void FindCellsToReveal((int i, int j) gridIndex, List<Cell> cellsToReveal)
    {
        var (i, j) = gridIndex;
        if (!IsValidIndex(gridIndex))
            return;

        var cell = this[i, j];
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