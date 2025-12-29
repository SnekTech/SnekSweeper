using System.Text.Json;
using System.Text.Json.Serialization;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;

namespace SnekSweeperCore.SaveLoad;

public record PlayerSaveData(
    MainSetting MainSetting,
    ActivatedCheatCodeSet ActivatedCheatCodeSet,
    History History);

[JsonSerializable(typeof(PlayerSaveDataDto))]
[JsonSerializable(typeof(List<string>))]
partial class PlayerSaveDataDtoSerializerContext : JsonSerializerContext;

public static class PlayerSaveDataJsonExtensions
{
    const string SaveFileName = "playerSaveData.json";
    static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    static readonly PlayerSaveDataDtoSerializerContext SerializerContext = new(SerializerOptions);

    extension(PlayerSaveData playerSaveData)
    {
        public static PlayerSaveData CreateEmpty() => new(new MainSetting(), new ActivatedCheatCodeSet(), new History());

        public void Save(string userDataDir)
        {
            var json = JsonSerializer.Serialize(playerSaveData.ToDto(), SerializerContext.PlayerSaveDataDto);
            File.WriteAllText(userDataDir.SaveFilePath, json);
        }

        public static PlayerSaveData? Load(string userDataDir)
        {
            PlayerSaveDataDto? loadedDto = null;
            try
            {
                var json = File.ReadAllText(userDataDir.SaveFilePath);
                loadedDto = JsonSerializer.Deserialize(json, SerializerContext.PlayerSaveDataDto);
            }
            catch (Exception)
            {
                // ignored, will create an empty save later
            }

            return loadedDto switch
            {
                not null => loadedDto.ToPlayerSaveData(),
                null => null,
            };
        }
    }

    extension(string userDataDir)
    {
        string SaveFilePath => Path.Combine(userDataDir, SaveFileName);
    }
}