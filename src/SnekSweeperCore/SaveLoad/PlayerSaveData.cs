using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad.SerializationService;

namespace SnekSweeperCore.SaveLoad;

public record PlayerSaveData(
    MainSetting MainSetting,
    ActivatedCheatCodeSet ActivatedCheatCodeSet,
    CurrentRunInfo CurrentRunInfo,
    History History);

public enum SaveFormat
{
    Json,
    MemoryPack,
}

public static class PlayerSaveDataExtensions
{
    static SerializationStrategy StrategyFor(SaveFormat format) => format switch
    {
        SaveFormat.Json => SerializationStrategy.Json,
        SaveFormat.MemoryPack => SerializationStrategy.MemoryPack,
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };

    extension(PlayerSaveData playerSaveData)
    {
        public static PlayerSaveData CreateEmpty() =>
            new(new MainSetting(), new ActivatedCheatCodeSet([]), new CurrentRunInfo(), new History([]));

        public void Save(string userDataDir, SaveFormat format = SaveFormat.Json) =>
            StrategyFor(format).Save(playerSaveData, userDataDir.ToDir());

        public Task SaveAsync(string userDataDir, SaveFormat format = SaveFormat.Json, CancellationToken ct = default) =>
            StrategyFor(format).SaveAsync(playerSaveData, userDataDir.ToDir(), ct);

        public static PlayerSaveData? Load(string userDataDir, SaveFormat format = SaveFormat.Json) =>
            StrategyFor(format).Load(userDataDir.ToDir());
    }

    extension(string saveDir)
    {
        SaveDir ToDir() => new(saveDir);
    }
}
