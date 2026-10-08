using System.Runtime.CompilerServices;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;

namespace SnekSweeperCore.LevelManagement;

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

        public LevelSetup ToSetup() => loadLevelSource switch
        {
            FromOngoingGame { OngoingGame: var ongoingGame } => new LevelSetup.Resume(ongoingGame),
            FromRunRecord
            {
                RunRecord:
                {
                    BombMatrix: var bombs,
                    StartIndex: var startIndex,
                },
            } => new LevelSetup.NewGame(new MineLayout.Fixed(bombs), startIndex),
            RegularStart
            {
                DifficultyData: var difficulty,
                Solvable: var solvable,
            } => new LevelSetup.NewGame(new MineLayout.Random(difficulty, solvable), null),
            _ => throw new SwitchExpressionException(),
        };
    }
}

public abstract record LevelSetup
{
    public sealed record NewGame(MineLayout Layout, GridIndex? RequiredStartIndex) : LevelSetup;
    public sealed record Resume(OngoingGame OngoingGame) : LevelSetup;
}

public static class LevelSetupExtensions
{
    extension(LevelSetup levelSetup)
    {
        public GridSize Size => levelSetup switch
        {
            LevelSetup.NewGame { Layout.Size: var size } => size,
            LevelSetup.Resume { OngoingGame.GridSnapshot.BombMatrix.Size: var size } => size,
            _ => throw new SwitchExpressionException(),
        };
    }
}