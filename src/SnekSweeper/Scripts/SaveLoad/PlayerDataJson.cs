using System.IO;
using System.Text.Json;
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
        return JsonSerializer.Deserialize<PlayerDataJson>(json) ?? new PlayerDataJson();
    }
}

public static class PlayerDataJsonExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = OS.IsDebugBuild() };

    public static void Save(this PlayerDataJson playerDataJson)
    {
        var json = JsonSerializer.Serialize(playerDataJson, SerializerOptions);
        File.WriteAllText(PlayerDataJson.SavePath, json);
    }

    public static bool Exists(this PlayerDataJson playerDataJson)
    {
        return File.Exists(PlayerDataJson.SavePath);
    }
}