using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnekSweeperCore.SaveLoad.SerializationService;

static class JsonSerializationService
{
    extension(SerializationStrategy)
    {
        internal static SerializationStrategy Json => new(SaveByJson, LoadByJson);
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

    public static SaveDataFn SaveByJson => (playerSaveData, saveDir) =>
    {
        var json = JsonSerializer.Serialize(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);
        File.WriteAllText(saveDir.Combine(SaveJsonFileName), json);
    };

    public static readonly LoadDataFn LoadByJson = saveDir =>
    {
        PlayerSaveDataDto? loadedDto = null;
        try
        {
            var json = File.ReadAllText(saveDir.Combine(SaveJsonFileName));
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