using SnekSweeper.CheatCodeSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper : Node
{
    private static PlayerDataJson _playerDataJson = null!;

    public override void _Ready()
    {
        CreateOrLoadPlayerData();

        SaveLoadEventBus.SaveRequested += SavePlayerData;
    }

    public override void _ExitTree()
    {
        SaveLoadEventBus.SaveRequested -= SavePlayerData;
    }

    private void CreateOrLoadPlayerData()
    {
        if (_playerDataJson.Exists())
        {
            _playerDataJson = PlayerDataJson.Load();
        }
        else
        {
            _playerDataJson = new PlayerDataJson();
            _playerDataJson.Save();
        }
    }

    private void SavePlayerData()
    {
        _playerDataJson.Save();
    }

    public static MainSetting MainSetting => _playerDataJson.MainSetting;
    public static History History => _playerDataJson.History;
    public static ActivatedCheatCodeSet ActivatedCheatCodeSet => _playerDataJson.ActivatedCheatCodeSet;
}