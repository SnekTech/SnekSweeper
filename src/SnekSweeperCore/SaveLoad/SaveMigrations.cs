namespace SnekSweeperCore.SaveLoad;

public sealed class SaveVersionNotSupportedException(int version)
    : Exception($"Save data version {version} is not supported.");

static class SaveMigrations
{
    // --- Loader entry points: null-safe, deserialized DTO → current domain ---

    internal static PlayerSaveData? MigrateAndMap(PlayerSaveDataDtoV1? v1) =>
        v1 is null ? null : MigrateToCurrent(v1).ToPlayerSaveData();

    internal static PlayerSaveData? MigrateAndMap(PlayerSaveDataDtoV2? v2) =>
        v2 is null ? null : MigrateToCurrent(v2).ToPlayerSaveData();

    // --- Chain entries: walk from a historical version up to current (V3) ---

    internal static PlayerSaveDataDtoV3 MigrateToCurrent(PlayerSaveDataDtoV1 v1) =>
        MigrateV2ToV3(MigrateV1ToV2(v1));

    internal static PlayerSaveDataDtoV3 MigrateToCurrent(PlayerSaveDataDtoV2 v2) =>
        MigrateV2ToV3(v2);

    // --- Individual pure steps (field-by-field copy) ---

    internal static PlayerSaveDataDtoV2 MigrateV1ToV2(PlayerSaveDataDtoV1 v1) =>
        new(
            new MainSettingDtoV2(
                v1.MainSetting.CurrentDifficultyKey,
                v1.MainSetting.CurrentSkinKey,
                v1.MainSetting.ComboRankDisplay,
                v1.MainSetting.GenerateSolvableGrid),
            new ActivatedCheatCodeSetDtoV2(v1.ActivatedCheatCodeSet.ActivatedSet),
            new CurrentRunInfoDtoV2(v1.CurrentRunInfo.GridSnapshot, v1.CurrentRunInfo.StartInfo),
            new HistoryDtoV2(v1.History.Records));

    internal static PlayerSaveDataDtoV3 MigrateV2ToV3(PlayerSaveDataDtoV2 v2) =>
        new(
            new MainSettingDtoV3(
                v2.MainSetting.CurrentDifficultyKey,
                v2.MainSetting.CurrentSkinKey,
                v2.MainSetting.ComboRankDisplay,
                v2.MainSetting.GenerateSolvableGrid),
            new ActivatedCheatCodeSetDtoV3(v2.ActivatedCheatCodeSet.ActivatedSet),
            new CurrentRunInfoDtoV3(v2.CurrentRunInfo.GridSnapshot, v2.CurrentRunInfo.StartInfo),
            new HistoryDtoV3(v2.History.Records));
}
