using System.IO;
using System.Text.Json;
using GodotGadgets.Extensions;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GameSettings;

namespace SnekSweeper.SaveLoad;

public class PlayerDataJson
{
    public static readonly string SavePath = Path.Combine(OS.GetUserDataDir(), "playerData.json");

    public MainSetting MainSetting { get; init; } = new();
    public ActivatedCheatCodeSet ActivatedCheatCodeSet { get; init; } = new();
    public History History { get; init; } = new();

    public static PlayerDataJson Load()
    {
        var json = File.ReadAllText(SavePath);
        PlayerDataJson? playerData = null;
        try
        {
            playerData = JsonSerializer.Deserialize(json, PlayerDataJsonExtensions.SerializerContext.PlayerDataJson);
        }
        catch (Exception jsonException)
        {
            jsonException.Message.DumpGd();
        }
        
        return playerData ?? new PlayerDataJson();
    }
}

public static class PlayerDataJsonExtensions
{
    static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = OS.IsDebugBuild() };
    internal static readonly PlayerDataSerializerContext SerializerContext = new(SerializerOptions);

    public static void Save(this PlayerDataJson playerDataJson)
    {
        var json = JsonSerializer.Serialize(playerDataJson, SerializerContext.PlayerDataJson);
        File.WriteAllText(PlayerDataJson.SavePath, json);
    }

    public static bool Exists(this PlayerDataJson playerDataJson)
    {
        return File.Exists(PlayerDataJson.SavePath);
    }
}