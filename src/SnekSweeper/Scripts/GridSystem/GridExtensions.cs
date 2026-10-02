using SnekSweeper.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

static class GridIndexExtensions
{
    extension(GridIndex index)
    {
        public Vector2 ToPosition(int cellSizePixels = HumbleCell.CellSizeInPixels)
        {
            var (i, j) = index;
            return new Vector2(j * cellSizePixels, i * cellSizePixels);
        }
    }

    extension(GridSize gridSize)
    {
        public Vector2 ToPixels(int cellSizePixels = HumbleCell.CellSizeInPixels)
        {
            var (rows, columns) = gridSize;
            return new Vector2(columns * cellSizePixels, rows * cellSizePixels);
        }
    }
}