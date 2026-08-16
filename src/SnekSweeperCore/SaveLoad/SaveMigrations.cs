using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.SaveLoad;

public sealed class SaveVersionNotSupportedException(int version)
    : Exception($"Save data version {version} is not supported.");

static class SaveMigrations
{
    // --- Loader entry points: null-safe, deserialized DTO → current domain ---

    internal static PlayerSaveData? MigrateAndMap(PlayerSaveDataDtoV1? v1) =>
        v1 is null ? null : MigrateToCurrent(v1).ToPlayerSaveData();

    // --- Chain entries: walk from a historical version up to current (V2) ---

    internal static PlayerSaveDataDtoV2 MigrateToCurrent(PlayerSaveDataDtoV1 v1) =>
        MigrateV1ToV2(v1);

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
