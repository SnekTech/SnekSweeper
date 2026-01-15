namespace SnekSweeperCore.SaveLoad.SerializationService;

public readonly record struct FilePath(string Value);

public readonly record struct SaveDir(string Value);

public delegate void SaveDataFn(PlayerSaveData playerSaveData, SaveDir saveDir, string fileName);

public delegate PlayerSaveData? LoadDataFn(SaveDir saveDir, string fileName);

record SerializationStrategy(SaveDataFn SaveDataFn, LoadDataFn LoadDataFn, string SaveFileName);

static class SerializationStrategyExtensions
{
    extension(SerializationStrategy strategy)
    {
        internal static SerializationStrategy MemoryPackWithJsonWriting =>
            new((playerSaveData, saveDir, _) =>
                {
                    SerializationStrategy.Json.Save(playerSaveData, saveDir);
                    SerializationStrategy.MemoryPack.Save(playerSaveData, saveDir);
                },
                (saveDir, _) => SerializationStrategy.MemoryPack.Load(saveDir)
                , "playerSave.debug");

        internal void Save(PlayerSaveData playerSaveData, SaveDir saveDir) =>
            strategy.SaveDataFn(playerSaveData, saveDir, strategy.SaveFileName);

        internal PlayerSaveData? Load(SaveDir saveDir) => strategy.LoadDataFn(saveDir, strategy.SaveFileName);
    }

    extension(SaveDir userDataDir)
    {
        internal FilePath Combine(string fileName) => new(Path.Combine(userDataDir.Value, fileName));
    }
}