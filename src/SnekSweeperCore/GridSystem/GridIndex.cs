namespace SnekSweeperCore.GridSystem;

public readonly record struct GridIndex(int I, int J);

public readonly record struct GridSize(int Rows, int Columns);

public static class GridIndexExtensions
{
    extension(GridIndex gridIndex)
    {
        public bool IsWithin(GridSize size)
        {
            var (i, j) = gridIndex;
            var (rows, columns) = size;
            return i >= 0 && i < rows && j >= 0 && j < columns;
        }

        public IEnumerable<GridIndex> GetNeighborIndicesWithin(GridSize gridSize) => NeighborOffsets.Select(offset =>
        {
            var (i, j) = gridIndex;
            var (offsetI, offsetJ) = offset;
            return new GridIndex(i + offsetI, j + offsetJ);
        }).Where(neighborIndex => neighborIndex.IsWithin(gridSize));
    }

    static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
    [
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1),
    ];
}