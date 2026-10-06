using System.Runtime.CompilerServices;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.GridSystem.LayMineStrategies;

namespace SnekSweeperCore.LevelManagement;

public abstract record MineLayout
{
    public sealed record Fixed(bool[,] Bombs) : MineLayout;
    public sealed record Random(GridDifficultyData Difficulty, bool Solvable) : MineLayout;
}

public static class MineLayoutExtensions
{
    extension(MineLayout layout)
    {
        public GridSize Size => layout switch
        {
            MineLayout.Fixed { Bombs.Size: var size } => size,
            MineLayout.Random { Difficulty.Size: var size } => size,
            _ => throw new SwitchExpressionException(),
        };

        public bool[,] Lay(GridIndex firstIndex) => layout switch
        {
            MineLayout.Fixed { Bombs: var bombs } => bombs,
            MineLayout.Random { Difficulty: var difficulty, Solvable: var solvable } => solvable
                ? LayMineStrategies.LayMineSolvable(difficulty, firstIndex)
                : LayMineStrategies.LayMineClassic(difficulty, firstIndex),
            _ => throw new SwitchExpressionException(),
        };
    }
}