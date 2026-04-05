using System.Text.Json;
using System.Text.Json.Serialization;
using GodotGadgets;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class JsonSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy Json =>
            new(SaveByJson, SaveByJsonAsync, LoadByJson, SaveJsonFileName);
    }

    const string SaveJsonFileName = "playerSaveData.json";

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
        var json = await playerSaveData.ToJsonAsync(ct);
        await FileOperations.SafeWriteAllTextAsync(saveDir.Combine(fileName).Value, json, ct);
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
        string ToJson() => JsonSerializer.Serialize(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);

        async Task<string> ToJsonAsync(CancellationToken ct = default)
        {
            await using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, playerSaveData.ToDto(),
                SerializerContext.PlayerSaveDataDto, ct);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            return await reader.ReadToEndAsync(ct);
        }

        static PlayerSaveData? FromJson(string json)
        {
            return JsonSerializer.Deserialize(json, SerializerContext.PlayerSaveDataDto)?.ToPlayerSaveData();
        }
    }
}

[JsonSerializable(typeof(PlayerSaveDataDto))]
[JsonSerializable(typeof(List<string>))]
partial class PlayerSaveDataDtoSerializerContext : JsonSerializerContext;