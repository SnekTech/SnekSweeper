using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class JsonSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy Json => new(SaveByJson, LoadByJson, SaveJsonFileName);
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

    public static SaveDataFn SaveByJson => (playerSaveData, filePath) =>
    {
        var json = JsonSerializer.Serialize(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);
        File.WriteAllText(filePath, json);
    };

    public static readonly LoadDataFn LoadByJson = filePath =>
    {
        PlayerSaveDataDto? loadedDto = null;
        try
        {
            var json = File.ReadAllText(filePath);
            loadedDto = JsonSerializer.Deserialize(json, SerializerContext.PlayerSaveDataDto);
        }
        catch (Exception)
        {
            // ignored, will create an empty save later
        }

        return loadedDto?.ToPlayerSaveData();
    };
}

[JsonSerializable(typeof(PlayerSaveDataDto))]
[JsonSerializable(typeof(List<string>))]
partial class PlayerSaveDataDtoSerializerContext : JsonSerializerContext;