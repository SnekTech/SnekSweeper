using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
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

public static class LevelLoading
{
    extension(LoadLevelSource loadLevelSource)
    {
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
            FromRunRecord fromRunRecord =>
                _ => LayMineStrategies.LayMineHardcoded(fromRunRecord.RunRecord.BombMatrix),
            _ => throw new SwitchExpressionException(),
        };

        public Grid CreateGrid(IHumbleGrid humbleGrid, GridEventBus gridEventBus, GridSkin gridSkin)
        {
            humbleGrid.HumbleCellsContainer.Clear();
            var cells = loadLevelSource switch
            {
                RegularStart regularStart => MatrixExtensions.Create(regularStart.DifficultyData.Size, gridIndex =>
                {
                    var humbleCell =
                        humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, gridSkin);
                    return new Cell(humbleCell, gridIndex);
                }),
                FromRunRecord fromRunRecord => fromRunRecord.RunRecord.BombMatrix.MapTo((_, gridIndex) =>
                {
                    var humbleCell =
                        humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, gridSkin);
                    return new Cell(humbleCell, gridIndex);
                }),
                _ => throw new SwitchExpressionException(),
            };

            return new Grid(humbleGrid, cells, gridEventBus);
        }
    }
}