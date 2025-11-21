using SnekSweeper.GridSystem;
using SnekSweeper.GridSystem.LayMineStrategies;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GameSettings;

public class MainSetting
{
    public GridDifficulty CurrentDifficulty { get; set; } = DifficultyFactory.Intermediate;
    public SkinKey CurrentSkinKey { get; set; } = SkinKey.Classic;
    public bool ComboRankDisplay { get; set; } = true;
    public LayMineStrategyName CurrentStrategyName { get; set; } = LayMineStrategyName.Solvable;
}