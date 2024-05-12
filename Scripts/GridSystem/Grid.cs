using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public class Grid
{
    private static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
    {
        new(-1, -1),
        new(0, -1),
        new(1, -1),
        new(-1, 0),
        new(1, 0),
        new(-1, 1),
        new(0, 1),
        new(1, 1),
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
            
            cell.Init();
        }
    }

    public Cell this[int i, int j] => _cells[i, j];
}