using SnekSweeper.Constants;

namespace SnekSweeper.GridSystem;

public readonly record struct GridIndex(int I, int J);

internal static class GridIndexExtensions
{
    extension(GridIndex index)
    {
        public Vector2 ToPosition(int cellSizePixels = CoreStats.CellSizePixels)
        {
            var (i, j) = index;
            return new Vector2(j * cellSizePixels, i * cellSizePixels);
        }
    }
}

public readonly record struct GridSize(int Rows, int Columns);