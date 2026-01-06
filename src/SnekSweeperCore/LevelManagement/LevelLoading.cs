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

public sealed record RegularStart(GridDifficultyData DifficultyData, bool Solvable) : LoadLevelSource;

public sealed record FromRunRecord(GameRunRecord RunRecord) : LoadLevelSource;

public static class LevelLoading
{
    extension(LoadLevelSource loadLevelSource)
    {
        public static LoadLevelSource CreateRegularStart(MainSetting mainSetting)
        {
            var difficulty = mainSetting.CurrentDifficultyKey.ToDifficulty().DifficultyData;
            var solvable = mainSetting.CurrentStrategyKey == LayMineStrategyKey.Solvable;
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
        
        Cell[,] CreateCells(IHumbleGrid humbleGrid, GridSkin skin) =>
            loadLevelSource switch
            {
                RegularStart regularStart => MatrixExtensions.Create(regularStart.DifficultyData.Size, gridIndex =>
                {
                    var humbleCell = humbleGrid.InstantiateHumbleCell(gridIndex, skin);
                    return new Cell(humbleCell, gridIndex);
                }),
                FromRunRecord fromRunRecord => fromRunRecord.RunRecord.BombMatrix.MapTo((hasBomb, gridIndex) =>
                {
                    var humbleCell = humbleGrid.InstantiateHumbleCell(gridIndex, skin);
                    return new Cell(humbleCell, gridIndex, hasBomb);
                }),
                _ => throw new SwitchExpressionException(),
            };

        public Grid CreateGrid(IHumbleGrid humbleGrid, GridEventBus eventBus, GridSkin skin)
        {
            var cells = loadLevelSource.CreateCells(humbleGrid, skin);
            return new Grid(humbleGrid, cells, eventBus);
        }
    }
}