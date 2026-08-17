using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GameSettings;

public record MainSetting(
    GridDifficultyKey CurrentDifficultyKey = GridDifficultyKey.Intermediate,
    SkinKey CurrentSkinKey = SkinKey.Classic,
    bool ComboRankDisplay = true,
    bool GenerateSolvableGrid = true);
