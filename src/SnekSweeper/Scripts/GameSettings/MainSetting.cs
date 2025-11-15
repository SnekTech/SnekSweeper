using SnekSweeper.GridSystem;
using SnekSweeper.GridSystem.LayMineStrategies;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GameSettings;

public class MainSetting
{
    public GridDifficulty CurrentDifficulty { get; set; } = DifficultyFactory.Intermediate;
    public SkinData CurrentSkin { get; set; } = SkinFactory.Classic;
    public bool ComboRankDisplay { get; set; } = true;
    public LayMineStrategyName CurrentStrategy { get; set; } = LayMineStrategyName.Solvable;
}