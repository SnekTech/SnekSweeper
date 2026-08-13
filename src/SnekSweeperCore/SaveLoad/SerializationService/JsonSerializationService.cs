using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SnekGameDevKit;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class JsonSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy Json =>
            new(SaveByJson, SaveByJsonAsync, LoadByJson, SaveJsonFileName);
    }

    const string SaveJsonFileName = "playerSaveData.json";
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
            var dataNode = JsonSerializer.SerializeToNode(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);
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

            var version = versionValue.GetValue<int>();
            var dto = JsonSerializer.Deserialize(dataNode.ToJsonString(), SerializerContext.PlayerSaveDataDto);
            return dto is null ? null : SaveMigrations.MigrateToCurrent(dto, version).ToPlayerSaveData();
        }
    }
}

[JsonSerializable(typeof(PlayerSaveDataDto))]
[JsonSerializable(typeof(int[][]))]
partial class PlayerSaveDataDtoSerializerContext : JsonSerializerContext;
