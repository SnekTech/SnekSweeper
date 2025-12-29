using Riok.Mapperly.Abstractions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.GridSystem.LayMineStrategies;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.SaveLoad;

record MainSettingDto(
    GridDifficultyKey CurrentDifficultyKey,
    SkinKey CurrentSkinKey,
    bool ComboRankDisplay,
    LayMineStrategyKey CurrentStrategyKey
);

record ActivatedCheatCodeSetDto(HashSet<CheatCodeKey> ActivatedSet);

record HistoryDto(List<GameRunRecord> Records);

record PlayerSaveDataDto(
    MainSettingDto MainSetting,
    ActivatedCheatCodeSetDto ActivatedCheatCodeSet,
    HistoryDto History);

[Mapper]
static partial class PlayerSaveDataMapper
{
    internal static partial PlayerSaveDataDto ToDto(this PlayerSaveData playerSaveData);
    internal static partial PlayerSaveData ToPlayerSaveData(this PlayerSaveDataDto dto);
}