using MineSweeperTools;
using SnekSweeperCore.GridSystem.Difficulty;

namespace SnekSweeperCore.GridSystem.LayMineStrategies;

static class LayMineStrategies
{
    internal static bool[,] LayMineClassic(GridDifficultyData difficultyData, GridIndex startIndex)
    {
        var ((rows, columns), bombCount) = difficultyData;

        var bombs1d = new bool[rows * columns];
        for (var i = 0; i < bombCount; i++)
        {
            bombs1d[i] = true;
        }

        Random.Shared.Shuffle(bombs1d);

        var bombs2d = new bool[rows, columns];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                bombs2d[i, j] = bombs1d[i * rows + j];
            }
        }

        var startIndexIsSafe = !bombs2d.At(startIndex);
        if (startIndexIsSafe) return bombs2d;

        // find the first safe cell in matrix
        // then exchange it with the start cell which has bomb
        var firstSafeIndex = bombs2d.Indices().First(index => !bombs2d.At(index));
        bombs2d.SetAt(startIndex, false);
        bombs2d.SetAt(firstSafeIndex, true);

        return bombs2d;
    }

    internal static bool[,] LayMineSolvable(GridDifficultyData difficultyData, GridIndex startIndex)
    {
        var (gridSize, bombCount) = difficultyData;
        var mat = LayMineEngine.LayMineSolvable(
            gridSize.Rows, gridSize.Columns, bombCount, startIndex.I, startIndex.J);
        return mat;
    }

    internal static bool[,] LayMineHardcoded(bool[,] bombs) => bombs;
}

public enum LayMineStrategyKey
{
    Classic,
    Solvable,
    Hardcoded,
}