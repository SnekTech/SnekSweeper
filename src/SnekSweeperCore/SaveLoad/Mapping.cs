using MemoryPack;
using Riok.Mapperly.Abstractions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.SaveLoad;

[MemoryPackable]
partial record MainSettingDto(
    GridDifficultyKey CurrentDifficultyKey,
    SkinKey CurrentSkinKey,
    bool ComboRankDisplay,
    bool GenerateSolvableGrid
);

[MemoryPackable]
partial record ActivatedCheatCodeSetDto(HashSet<CheatCodeKey> ActivatedSet);

[MemoryPackable]
partial record CurrentRunInfoDto(GridSnapshot? GridSnapshot, RunStartInfo StartInfo);

[MemoryPackable]
partial record HistoryDto(List<GameRunRecord> Records);

[MemoryPackable]
partial record PlayerSaveDataDto(
    MainSettingDto MainSetting,
    ActivatedCheatCodeSetDto ActivatedCheatCodeSet,
    CurrentRunInfoDto CurrentRunInfo,
    HistoryDto History);

[Mapper]
static partial class PlayerSaveDataMapper
{
    internal static partial PlayerSaveDataDto ToDto(this PlayerSaveData playerSaveData);
    internal static partial PlayerSaveData ToPlayerSaveData(this PlayerSaveDataDto dto);
}