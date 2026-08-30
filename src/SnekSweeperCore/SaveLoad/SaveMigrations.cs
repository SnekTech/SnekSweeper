using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.SaveLoad;

public sealed class SaveVersionNotSupportedException(int version)
    : Exception($"Save data version {version} is not supported.");

static class SaveMigrations
{
    // --- Loader entry points: null-safe, deserialized DTO → current domain ---

    internal static PlayerSaveData? MigrateV1AndMap(PlayerSaveDataDtoV1? v1) =>
        v1 is null ? null : MigrateV1ToV3(v1).ToPlayerSaveData();

    internal static PlayerSaveData? MigrateV2AndMap(PlayerSaveDataDtoV2? v2) =>
        v2 is null ? null : MigrateV2ToV3(v2).ToPlayerSaveData();

    // --- Chain entries: walk from a historical version up to current (V3) ---

    internal static PlayerSaveDataDtoV3 MigrateV1ToV3(PlayerSaveDataDtoV1 v1) =>
        MigrateV2ToV3(MigrateV1ToV2(v1));

    internal static PlayerSaveDataDtoV3 MigrateV2ToV3(PlayerSaveDataDtoV2 v2)
    {
        var snapshot = v2.CurrentRunInfo.GridSnapshot;
        return new PlayerSaveDataDtoV3(
            v2.MainSetting,
            v2.ActivatedCheatCodeSet,
            new CurrentRunInfoDtoV3(snapshot is not null
                ? new OngoingGame(snapshot, v2.CurrentRunInfo.StartInfo)
                : null),
            v2.History);
    }

    // --- Individual pure steps ---

    internal static PlayerSaveDataDtoV2 MigrateV1ToV2(PlayerSaveDataDtoV1 v1) =>
        new(
            new MainSettingDtoV2(
                v1.MainSetting.CurrentDifficultyKey,
                v1.MainSetting.CurrentSkinKey,
                v1.MainSetting.ComboRankDisplay,
                v1.MainSetting.GenerateSolvableGrid),
            new ActivatedCheatCodeSetDtoV2(v1.ActivatedCheatCodeSet.ActivatedSet),
            new CurrentRunInfoDtoV2(MigrateGridSnapshot(v1.CurrentRunInfo.GridSnapshot), v1.CurrentRunInfo.StartInfo),
            new HistoryDtoV2(v1.History.Records));

    static GridSnapshot? MigrateGridSnapshot(GridSnapshotV1? v1) =>
        v1 is null ? null : new GridSnapshot(MatrixExtensions.FromJagged(v1.SnapshotStates), v1.BombMatrix);
}
