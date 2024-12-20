namespace SnekSweeper.GridSystem;

public interface IGridDifficulty
{
    string Name { get; }
    (int rows, int columns) Size { get; }
    float BombPercent { get; }
}