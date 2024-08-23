using System;
using Godot;

namespace SnekSweeper.SaveLoad;

public partial class PlayerData : Resource
{
    private const string SavePathBase = "user://playerData";

    [Export]
    public int CurrentDifficultyIndex = 0;

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