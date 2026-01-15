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

    public static SaveDataFn SaveByJson => (playerSaveData, saveDir, fileName) =>
    {
        var json = JsonSerializer.Serialize(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);
        File.WriteAllText(saveDir.Combine(fileName).Value, json);
    };

    public static readonly LoadDataFn LoadByJson = (saveDir, fileName) =>
    {
        PlayerSaveDataDto? loadedDto = null;
        try
        {
            var json = File.ReadAllText(saveDir.Combine(fileName).Value);
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