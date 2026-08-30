using MemoryPack;
using Riok.Mapperly.Abstractions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.SaveLoad;

// --- Historical version 1 ---
[MemoryPackable]
partial record MainSettingDtoV1(
    GridDifficultyKey CurrentDifficultyKey,
    SkinKey CurrentSkinKey,
    bool ComboRankDisplay,
    bool GenerateSolvableGrid
);

[MemoryPackable]
partial record ActivatedCheatCodeSetDtoV1(HashSet<CheatCodeKey> ActivatedSet);

[MemoryPackable]
partial record GridSnapshotV1(CellSnapshotState[][] SnapshotStates, bool[,] BombMatrix);

[MemoryPackable]
partial record CurrentRunInfoDtoV1(GridSnapshotV1? GridSnapshot, RunStartInfo StartInfo);

[MemoryPackable]
partial record HistoryDtoV1(List<GameRunRecord> Records);

[MemoryPackable]
partial record PlayerSaveDataDtoV1(
    MainSettingDtoV1 MainSetting,
    ActivatedCheatCodeSetDtoV1 ActivatedCheatCodeSet,
    CurrentRunInfoDtoV1 CurrentRunInfo,
    HistoryDtoV1 History);

// --- Historical version 2 ---
[MemoryPackable]
partial record MainSettingDtoV2(
    GridDifficultyKey CurrentDifficultyKey,
    SkinKey CurrentSkinKey,
    bool ComboRankDisplay,
    bool GenerateSolvableGrid
);

[MemoryPackable]
partial record ActivatedCheatCodeSetDtoV2(HashSet<CheatCodeKey> ActivatedSet);

[MemoryPackable]
partial record CurrentRunInfoDtoV2(GridSnapshot? GridSnapshot, RunStartInfo StartInfo);

[MemoryPackable]
partial record HistoryDtoV2(List<GameRunRecord> Records);

[MemoryPackable]
partial record PlayerSaveDataDtoV2(
    MainSettingDtoV2 MainSetting,
    ActivatedCheatCodeSetDtoV2 ActivatedCheatCodeSet,
    CurrentRunInfoDtoV2 CurrentRunInfo,
    HistoryDtoV2 History);

// --- Current version 3 ---
[MemoryPackable]
partial record CurrentRunInfoDtoV3(OngoingGame? OngoingGame);

[MemoryPackable]
partial record PlayerSaveDataDtoV3(
    MainSettingDtoV2 MainSetting,
    ActivatedCheatCodeSetDtoV2 ActivatedCheatCodeSet,
    CurrentRunInfoDtoV3 CurrentRunInfo,
    HistoryDtoV2 History);

[Mapper]
static partial class PlayerSaveDataMapper
{
    internal static partial PlayerSaveDataDtoV3 ToDto(this PlayerSaveData playerSaveData);
    internal static partial PlayerSaveData ToPlayerSaveData(this PlayerSaveDataDtoV3 dto);
}
