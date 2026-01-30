using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GameSettings;

public class MainSetting
{
    public GridDifficultyKey CurrentDifficultyKey { get; set; } = GridDifficultyKey.Intermediate;
    public SkinKey CurrentSkinKey { get; set; } = SkinKey.Classic;
    public bool ComboRankDisplay { get; set; } = true;
    public bool GenerateSolvableGrid { get; set; } = true;
}