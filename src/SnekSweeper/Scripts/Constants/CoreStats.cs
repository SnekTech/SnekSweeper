namespace SnekSweeper.Constants;

public static class CoreStats
{
    public const int CellSizePixels = 16;

    public static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
    [
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1)
    ];
}