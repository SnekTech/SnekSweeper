using SnekSweeper.GridSystem;
using SnekSweeper.GridSystem.LayMineStrategies;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GameSettings;

public class MainSetting
{
    public GridDifficultyKey CurrentDifficultyKey { get; set; } = GridDifficultyKey.Intermediate;
    public SkinKey CurrentSkinKey { get; set; } = SkinKey.Classic;
    public bool ComboRankDisplay { get; set; } = true;
    public LayMineStrategyKey CurrentStrategyKey { get; set; } = LayMineStrategyKey.Solvable;
}