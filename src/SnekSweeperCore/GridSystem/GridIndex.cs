namespace SnekSweeperCore.GridSystem;

public readonly record struct GridIndex(int I, int J)
{
    public static readonly (int offsetI, int offsetJ)[] NeighborOffsets =
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

public readonly record struct GridSize(int Rows, int Columns);