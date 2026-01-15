namespace SnekSweeperCore.SaveLoad.SerializationService;

public delegate void SaveDataFn(PlayerSaveData playerSaveData, string saveDir);

public delegate PlayerSaveData? LoadDataFn(string saveDir);

record SerializationStrategy(SaveDataFn SaveDataFn, LoadDataFn LoadDataFn);

static class SerializationStrategyExtensions
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy MemoryPackWithJsonWriting =>
            new((playerSaveData, saveDir) =>
                {
                    SerializationStrategy.Json.SaveDataFn(playerSaveData, saveDir);
                    SerializationStrategy.MemoryPack.SaveDataFn(playerSaveData, saveDir);
                },
                saveDir => SerializationStrategy.MemoryPack.LoadDataFn(saveDir)
            );
    }

    extension(string userDataDir)
    {
        internal string Combine(string fileName) => Path.Combine(userDataDir, fileName);
    }
}