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

public abstract record LoadLevelSource(GridSkin Skin);

public sealed record RegularStart(
    GridDifficultyData DifficultyData,
    bool Solvable,
    GridSkin Skin
) : LoadLevelSource(Skin);

public sealed record FromRunRecord(
    GameRunRecord RunRecord,
    GridSkin Skin
) : LoadLevelSource(Skin);

public static class LevelLoading
{
    extension(LoadLevelSource loadLevelSource)
    {
        public static LoadLevelSource CreateRegularStart(MainSetting mainSetting)
        {
            var difficulty = mainSetting.CurrentDifficultyKey.ToDifficulty().DifficultyData;
            var solvable = mainSetting.GenerateSolvableGrid;
            var skin = mainSetting.CurrentSkinKey.ToSkin();
            return new RegularStart(difficulty, solvable, skin);
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

        public Grid CreateGrid(IHumbleGrid humbleGrid, GridEventBus gridEventBus)
        {
            humbleGrid.HumbleCellsContainer.Clear();
            var cells = loadLevelSource switch
            {
                RegularStart regularStart => MatrixExtensions.Create(regularStart.DifficultyData.Size, gridIndex =>
                {
                    var humbleCell = humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, regularStart.Skin);
                    return new Cell(humbleCell, gridIndex);
                }),
                FromRunRecord fromRunRecord => fromRunRecord.RunRecord.BombMatrix.MapTo((_, gridIndex) =>
                {
                    var humbleCell = humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, fromRunRecord.Skin);
                    return new Cell(humbleCell, gridIndex);
                }),
                _ => throw new SwitchExpressionException(),
            };

            return new Grid(humbleGrid, cells, gridEventBus);
        }
    }
}