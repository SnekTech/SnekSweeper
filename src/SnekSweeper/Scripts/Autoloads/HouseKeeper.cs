using SnekSweeper.CheatCodeSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper : Node
{
    static PlayerDataJson _currentPlayerSaveData = null!;

    public override void _Ready()
    {
        CreateOrLoadPlayerData();

        SaveLoadEventBus.SaveRequested += SaveCurrentPlayerData;
    }

    public override void _ExitTree()
    {
        SaveLoadEventBus.SaveRequested -= SaveCurrentPlayerData;
    }

    static void CreateOrLoadPlayerData()
    {
        if (PlayerDataJson.TryLoad(out var loaded))
        {
            _currentPlayerSaveData = loaded;
        }
        else
        {
            _currentPlayerSaveData = PlayerDataJson.CreateEmpty();
            SaveCurrentPlayerData();
        }
    }

    static void SaveCurrentPlayerData()
    {
        _currentPlayerSaveData.Save();
    }

    internal static MainSetting MainSetting => _currentPlayerSaveData.MainSetting;
    internal static History History => _currentPlayerSaveData.History;
    internal static ActivatedCheatCodeSet ActivatedCheatCodeSet => _currentPlayerSaveData.ActivatedCheatCodeSet;
}