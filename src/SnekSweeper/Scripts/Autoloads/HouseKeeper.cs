using GodotGadgets.Extensions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper : Node
{
    static PlayerSaveData _currentPlayerSaveData = null!;

    public override void _Ready()
    {
        _currentPlayerSaveData = CreateOrLoadPlayerData();
    }

    static PlayerSaveData CreateOrLoadPlayerData()
    {
        var loaded = PlayerSaveData.Load(OS.GetUserDataDir());
        if (loaded is not null) return loaded;

        "cannot load player save data from disk, will create an empty new one".DumpGd();
        return PlayerSaveData.CreateEmpty();
    }

    public static void SaveCurrentPlayerData()
    {
        _currentPlayerSaveData.Save(OS.GetUserDataDir());
    }

    internal static MainSetting MainSetting => _currentPlayerSaveData.MainSetting;
    internal static History History => _currentPlayerSaveData.History;
    internal static ActivatedCheatCodeSet ActivatedCheatCodeSet => _currentPlayerSaveData.ActivatedCheatCodeSet;
}