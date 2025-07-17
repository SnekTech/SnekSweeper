using SnekSweeper.CheatCode;
using SnekSweeper.GameHistory;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper : Node
{
    [Export]
    private CheatCodeCollection cheatCodeCollection = null!;

    private static PlayerData _playerData = null!;
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
        if (PlayerData.Exists)
        {
            _playerData = PlayerData.Load();
        }
        else
        {
            _playerData = new PlayerData();
            _playerData.CheatCodeSaveData.Init(cheatCodeCollection);
            _playerData.Save();
        }

        if (_playerDataJson.Exists())
        {
            _playerDataJson = PlayerDataJson.Load();
        }
        else
        {
            _playerDataJson = new PlayerDataJson();
            _playerDataJson.Save();
        }

        MainSetting.CurrentSkin = _playerDataJson.CurrentSkin;
    }

    private void SavePlayerData()
    {
        _playerData.Save();

        _playerDataJson.CurrentSkin = MainSetting.CurrentSkin;
        _playerDataJson.Save();
    }

    public static MainSetting MainSetting => _playerData.MainSetting;

    public static History History => _playerData.History;
    public static CheatCodeSaveData CheatCodeSaveData => _playerData.CheatCodeSaveData;
}