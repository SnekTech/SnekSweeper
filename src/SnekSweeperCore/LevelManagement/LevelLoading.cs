using System.Runtime.CompilerServices;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.GridSystem.LayMineStrategies;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.LevelManagement;

public delegate bool[,] LayMineFn(GridIndex startIndex);
public abstract record LoadLevelSource;

public sealed record RegularStart(
    GridDifficultyData DifficultyData,
    bool Solvable
) : LoadLevelSource;

public sealed record FromRunRecord(
    GameRunRecord RunRecord
) : LoadLevelSource;

public sealed record FromOngoingGame(OngoingGame OngoingGame) : LoadLevelSource;

public static class LevelLoading
{
    extension(LoadLevelSource loadLevelSource)
    {
        public static LoadLevelSource CreateDefaultRegularStart() =>
            new RegularStart(GridDifficultyKey.Intermediate.ToDifficulty().DifficultyData, true);

        public static LoadLevelSource CreateRegularStart(MainSetting mainSetting)
        {
            var difficulty = mainSetting.CurrentDifficultyKey.ToDifficulty().DifficultyData;
            var solvable = mainSetting.GenerateSolvableGrid;
            return new RegularStart(difficulty, solvable);
        }

        public LayMineFn LayMineFn => loadLevelSource switch
        {
            RegularStart regularStart => regularStart.Solvable
                ? startIndex => LayMineStrategies.LayMineSolvable(regularStart.DifficultyData, startIndex)
                : startIndex => LayMineStrategies.LayMineClassic(regularStart.DifficultyData, startIndex),
            FromRunRecord fromRunRecord => _ => fromRunRecord.RunRecord.BombMatrix,
            FromOngoingGame fromOngoingGame => _ => fromOngoingGame.OngoingGame.GridSnapshot.BombMatrix,
            _ => throw new SwitchExpressionException(),
        };

        public Grid CreateGrid(ICellFactory cellFactory, GridSkin gridSkin, GridEventBus gridEventBus,
            CommandInvoker commandInvoker)
        {
            return Grid.Create(cellFactory, loadLevelSource.GetGridSize(), gridSkin, gridEventBus, commandInvoker);
        }

        GridSize GetGridSize() => loadLevelSource switch
        {
            RegularStart regularStart => regularStart.DifficultyData.Size,
            FromRunRecord fromRunRecord => fromRunRecord.RunRecord.BombMatrix.Size,
            FromOngoingGame fromOngoingGame => fromOngoingGame.OngoingGame.GridSnapshot.BombMatrix.Size,
            _ => throw new SwitchExpressionException(),
        };
    }
}
