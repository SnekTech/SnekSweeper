using SnekSweeper.GridSystem.Difficulty;
using SnekSweeper.NativeTools;

namespace SnekSweeper.GridSystem.LayMineStrategies;

public sealed class Solvable(GridDifficultyData difficultyData) : ILayMineStrategy
{
    public bool[,] Generate(GridIndex startIndex)
    {
        var (gridSize, bombCount) = difficultyData;
        var mat = LayMineEngine.LayMineSolvable(
            gridSize.Rows, gridSize.Columns, bombCount, startIndex.I, startIndex.J);
        return mat;
    }
}