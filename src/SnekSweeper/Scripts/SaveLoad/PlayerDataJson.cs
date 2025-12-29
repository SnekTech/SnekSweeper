using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using GodotGadgets.Extensions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;

namespace SnekSweeper.SaveLoad;

record PlayerDataJson(MainSetting MainSetting, ActivatedCheatCodeSet ActivatedCheatCodeSet, History History);

static class PlayerDataJsonExtensions
{
    const string SaveFileName = "playerData.json";
    static readonly string SavePath = Path.Combine(OS.GetUserDataDir(), SaveFileName);

    static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = OS.IsDebugBuild() };
    internal static readonly PlayerDataSerializerContext SerializerContext = new(SerializerOptions);

    extension(PlayerDataJson playerDataJson)
    {
        internal static PlayerDataJson CreateEmpty() =>
            new(new MainSetting(), new ActivatedCheatCodeSet(), new History());

        internal void Save()
        {
            var json = JsonSerializer.Serialize(playerDataJson, SerializerContext.PlayerDataJson);
            File.WriteAllText(SavePath, json);
        }

        internal static bool TryLoad([MaybeNullWhen(false)] out PlayerDataJson outPlayerData)
        {
            PlayerDataJson? loaded = null;

            try
            {
                var json = File.ReadAllText(SavePath);
                loaded = JsonSerializer.Deserialize(json, SerializerContext.PlayerDataJson);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                fileNotFoundException.Message.DumpGd();
                outPlayerData = null;
                return false;
            }
            catch (Exception e)
            {
                e.DumpGd();
                if (OS.IsDebugBuild())
                {
                    throw;
                }
            }

            outPlayerData = loaded;
            return loaded != null;
        }
    }
}