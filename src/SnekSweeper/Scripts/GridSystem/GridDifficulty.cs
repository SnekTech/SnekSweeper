namespace SnekSweeper.GridSystem;

public class GridDifficulty : IGridDifficulty
{
    public GridDifficulty(string name, (int rows, int columns) size, float bombPercent)
    {
        Name = name;
        Size = size;
        BombPercent = bombPercent;
    }

    public string Name { get; }

    public (int rows, int columns) Size { get; }

    public float BombPercent { get; }
}