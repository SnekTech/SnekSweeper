using MemoryPack;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class MemoryPackSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy MemoryPack => new(SaveByMemoryPack, LoadByMemoryPack);
    }

    const string SaveBinaryFileName = "playerSaveData.bin";

    public static SaveDataFn SaveByMemoryPack => (playerSaveData, saveDir) =>
    {
        var bin = MemoryPackSerializer.Serialize(playerSaveData.ToDto());
        File.WriteAllBytes(saveDir.Combine(SaveBinaryFileName), bin);
    };

    public static readonly LoadDataFn LoadByMemoryPack = saveDir =>
    {
        PlayerSaveData? loadedPlayerData = null;
        try
        {
            var bin = File.ReadAllBytes(saveDir.Combine(SaveBinaryFileName));
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