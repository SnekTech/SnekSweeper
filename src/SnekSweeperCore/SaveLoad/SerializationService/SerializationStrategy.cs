namespace SnekSweeperCore.SaveLoad.SerializationService;

public readonly record struct FilePath(string Value);

public readonly record struct SaveDir(string Value);

public delegate void SaveDataFn(PlayerSaveData playerSaveData, SaveDir saveDir, string fileName);

public delegate Task SaveDataAsyncFn(PlayerSaveData playerSaveData, SaveDir saveDir, string fileName,
    CancellationToken ct = default);

public delegate PlayerSaveData? LoadDataFn(SaveDir saveDir, string fileName);

record SerializationStrategy(
    SaveDataFn SaveDataFn,
    SaveDataAsyncFn SaveDataAsyncFn,
    LoadDataFn LoadDataFn,
    string SaveFileName);

static class SerializationStrategyExtensions
{
    extension(SerializationStrategy strategy)
    {
        internal void Save(PlayerSaveData playerSaveData, SaveDir saveDir) =>
            strategy.SaveDataFn(playerSaveData, saveDir, strategy.SaveFileName);

        internal Task SaveAsync(PlayerSaveData playerSaveData, SaveDir saveDir, CancellationToken ct = default) =>
            strategy.SaveDataAsyncFn(playerSaveData, saveDir, strategy.SaveFileName, ct);

        internal PlayerSaveData? Load(SaveDir saveDir) => strategy.LoadDataFn(saveDir, strategy.SaveFileName);
    }

    extension(SaveDir userDataDir)
    {
        internal FilePath Combine(string fileName) => new(Path.Combine(userDataDir.Value, fileName));
    }
}