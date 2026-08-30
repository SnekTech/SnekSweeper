using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SnekGameDevKit;
using SnekSweeperCore.SaveLoad.CustomJsonConverter;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class JsonSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy Json =>
            new(SaveByJson, SaveByJsonAsync, LoadByJson, SaveFileNames.Json);
    }

    const string VersionPropertyName = "version";
    const string DataPropertyName = "data";

    static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        options.Converters.Add(new Mat2DConverter());
        options.Converters.Add(new CellSnapshotState2DConverter());
        return options;
    }

    static readonly PlayerSaveDataDtoSerializerContext SerializerContext = new(SerializerOptions);

    static readonly SaveDataFn SaveByJson = (playerSaveData, saveDir, fileName) =>
    {
        File.WriteAllText(saveDir.Combine(fileName).Value, playerSaveData.ToJson());
    };

    static async Task SaveByJsonAsync(PlayerSaveData playerSaveData, SaveDir saveDir, string fileName,
        CancellationToken ct = default)
    {
        await FileOperations.SafeWriteAllTextAsync(saveDir.Combine(fileName).Value, playerSaveData.ToJson(), ct);
    }

    public static readonly LoadDataFn LoadByJson = (saveDir, fileName) =>
    {
        PlayerSaveData? playerSaveData = null;
        try
        {
            var json = File.ReadAllText(saveDir.Combine(fileName).Value);
            playerSaveData = PlayerSaveData.FromJson(json);
        }
        catch (Exception)
        {
            // ignored, will create an empty save later
        }

        return playerSaveData;
    };

    extension(PlayerSaveData playerSaveData)
    {
        string ToJson()
        {
            var dataNode = JsonSerializer.SerializeToNode(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDtoV3);
            var envelope = new JsonObject
            {
                [VersionPropertyName] = SaveVersion.Current,
                [DataPropertyName] = dataNode,
            };
            return envelope.ToJsonString(SerializerOptions);
        }

        static PlayerSaveData? FromJson(string json)
        {
            if (JsonNode.Parse(json) is not JsonObject root) return null;
            if (root[VersionPropertyName] is not JsonValue versionValue) return null;
            if (root[DataPropertyName] is not { } dataNode) return null;

            return versionValue.GetValue<int>() switch
            {
                1 => SaveMigrations.MigrateV1AndMap(JsonSerializer.Deserialize(dataNode.ToJsonString(), SerializerContext.PlayerSaveDataDtoV1)),
                2 => SaveMigrations.MigrateV2AndMap(JsonSerializer.Deserialize(dataNode.ToJsonString(), SerializerContext.PlayerSaveDataDtoV2)),
                SaveVersion.Current => JsonSerializer.Deserialize(dataNode.ToJsonString(), SerializerContext.PlayerSaveDataDtoV3)?.ToPlayerSaveData(),
                _ => throw new SaveVersionNotSupportedException(versionValue.GetValue<int>()),
            };
        }
    }
}

[JsonSerializable(typeof(PlayerSaveDataDtoV1))]
[JsonSerializable(typeof(PlayerSaveDataDtoV2))]
[JsonSerializable(typeof(PlayerSaveDataDtoV3))]
[JsonSerializable(typeof(int[][]))]
partial class PlayerSaveDataDtoSerializerContext : JsonSerializerContext;
