using SnekSweeper.NativeTools;

namespace SnekSweeper.GridSystem;

public static class BombMatrix
{
    public static bool[,] GenerateSolvable(GridDifficultyData gridDifficultyData, GridIndex startIndex,
        int maxTimes = 100_0000)
    {
        var (gridSize, bombCount) = gridDifficultyData;
        var mat = LayMineEngine.LayMineSolvable(gridSize.Row, gridSize.Columns, bombCount, startIndex.I, startIndex.J,
            maxTimes);
        return mat;
    }
}