using SnekSweeper.CheatCodeSystem;
using SnekSweeper.SaveLoad;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;

namespace SnekSweeper.Autoloads;

public partial class HouseKeeper : Node
{
    static PlayerDataJson _currentPlayerSaveData = null!;

    public override void _Ready()
    {
        CreateOrLoadPlayerData();
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

    public static void SaveCurrentPlayerData()
    {
        _currentPlayerSaveData.Save();
    }

    internal static MainSetting MainSetting => _currentPlayerSaveData.MainSetting;
    internal static History History => _currentPlayerSaveData.History;
    internal static ActivatedCheatCodeSet ActivatedCheatCodeSet => _currentPlayerSaveData.ActivatedCheatCodeSet;
}