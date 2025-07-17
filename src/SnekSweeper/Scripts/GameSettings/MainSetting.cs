using SnekSweeper.GridSystem;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.GameSettings;

public class MainSetting
{
    public GridDifficulty CurrentDifficulty { get; set; } = DifficultyFactory.Medium;

    public SkinData CurrentSkin { get; set; } = SkinFactory.Classic;
}