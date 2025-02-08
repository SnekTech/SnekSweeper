using System;
using Godot;
using SnekSweeper.CheatCode;
using SnekSweeper.GameHistory;
using SnekSweeper.GameSettings;

namespace SnekSweeper.SaveLoad;

public partial class PlayerData : Resource
{
    private const string SavePathBase = "user://playerData";

    [Export]
    public MainSetting MainSetting { get; private set; } = new();
    
    [Export]
    public History History { get; private set; } = new();

    [Export] public CheatCodeSaveData CheatCodeSaveData { get; private set; } = new();

    public void Save()
    {
        ResourceSaver.Save(this, SavePath);
    }

    public static bool Exists => ResourceLoader.Exists(SavePath);

    public static PlayerData Load()
    {
        return ResourceLoader.Load(SavePath) as PlayerData ??
               throw new InvalidOperationException("no save data to be loaded");
    }

    private static string SavePath => SavePathBase + (OS.IsDebugBuild() ? ".tres" : ".res");
}