using MemoryPack;
using Riok.Mapperly.Abstractions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.GridSystem.LayMineStrategies;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.SaveLoad;

[MemoryPackable]
partial record MainSettingDto(
    GridDifficultyKey CurrentDifficultyKey,
    SkinKey CurrentSkinKey,
    bool ComboRankDisplay,
    LayMineStrategyKey CurrentStrategyKey
);

[MemoryPackable]
partial record ActivatedCheatCodeSetDto(HashSet<CheatCodeKey> ActivatedSet);

[MemoryPackable]
partial record HistoryDto(List<GameRunRecord> Records);

[MemoryPackable]
partial record PlayerSaveDataDto(
    MainSettingDto MainSetting,
    ActivatedCheatCodeSetDto ActivatedCheatCodeSet,
    HistoryDto History);

[Mapper]
static partial class PlayerSaveDataMapper
{
    internal static partial PlayerSaveDataDto ToDto(this PlayerSaveData playerSaveData);
    internal static partial PlayerSaveData ToPlayerSaveData(this PlayerSaveDataDto dto);
}