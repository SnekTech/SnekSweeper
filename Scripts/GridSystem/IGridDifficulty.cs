namespace SnekSweeper.GridSystem;

public interface IGridDifficulty
{
    (int rows, int columns) Size { get; }
    float BombPercent { get; }
}