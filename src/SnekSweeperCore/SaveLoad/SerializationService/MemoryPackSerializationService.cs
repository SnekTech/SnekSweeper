using MemoryPack;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class MemoryPackSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy MemoryPack => new(SaveByMemoryPack, SaveByMemoryPackAsync,
            LoadByMemoryPack, SaveFileNames.Binary);
    }

    public static SaveDataFn SaveByMemoryPack => (playerSaveData, saveDir, fileName) =>
    {
        var bin = MemoryPackSerializer.Serialize(ToEnvelope(playerSaveData));
        File.WriteAllBytes(saveDir.Combine(fileName).Value, bin);
    };

    static readonly SaveDataAsyncFn SaveByMemoryPackAsync = (playerSaveData, saveDir, fileName, ct) =>
    {
        var bin = MemoryPackSerializer.Serialize(ToEnvelope(playerSaveData));
        return File.WriteAllBytesAsync(saveDir.Combine(fileName).Value, bin, ct);
    };

    static BinarySaveEnvelope ToEnvelope(PlayerSaveData playerSaveData) =>
        new(SaveVersion.Current, MemoryPackSerializer.Serialize(playerSaveData.ToDto()));

    public static readonly LoadDataFn LoadByMemoryPack = (saveDir, fileName) =>
    {
        PlayerSaveData? loadedPlayerData = null;
        try
        {
            var bin = File.ReadAllBytes(saveDir.Combine(fileName).Value);
            loadedPlayerData = PlayerSaveData.FromBinary(bin);
        }
        catch (Exception)
        {
            // ignored, will create an empty save later
        }

        return loadedPlayerData;
    };

    extension(PlayerSaveData playerSaveData)
    {
        static PlayerSaveData? FromBinary(byte[] bin)
        {
            var envelope = MemoryPackSerializer.Deserialize<BinarySaveEnvelope>(bin);
            if (envelope is null) return null;

            return envelope.Version switch
            {
                1 => SaveMigrations.MigrateAndMap(MemoryPackSerializer.Deserialize<PlayerSaveDataDtoV1>(envelope.Payload)),
                SaveVersion.Current => MemoryPackSerializer.Deserialize<PlayerSaveDataDtoV2>(envelope.Payload)?.ToPlayerSaveData(),
                _ => throw new SaveVersionNotSupportedException(envelope.Version),
            };
        }
    }
}
