using Godot;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper: Node
{
    private static PlayerData _playerData = null!;
    
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
            _playerData.Save();
        }
    }

    private void SavePlayerData()
    {
        _playerData.Save();
    }
    
    public static MainSetting MainSetting => _playerData.MainSetting;
}