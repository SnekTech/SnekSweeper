using MemoryPack;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class MemoryPackSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy MemoryPack => new(SaveByMemoryPack, LoadByMemoryPack, SaveBinaryFileName);
    }

    const string SaveBinaryFileName = "playerSaveData.bin";

    public static SaveDataFn SaveByMemoryPack => (playerSaveData, saveDir, fileName) =>
    {
        var bin = MemoryPackSerializer.Serialize(playerSaveData.ToDto());
        File.WriteAllBytes(saveDir.Combine(fileName).Value, bin);
    };

    public static readonly LoadDataFn LoadByMemoryPack = (saveDir, fileName) =>
    {
        PlayerSaveData? loadedPlayerData = null;
        try
        {
            var bin = File.ReadAllBytes(saveDir.Combine(fileName).Value);
            var dto = MemoryPackSerializer.Deserialize<PlayerSaveDataDto>(bin);
            loadedPlayerData = dto?.ToPlayerSaveData();
        }
        catch (Exception)
        {
            // ignored, will create an empty save later
        }

        return loadedPlayerData;
    };
}