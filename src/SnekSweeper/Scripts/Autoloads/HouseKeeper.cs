using GodotGadgets.Extensions;
using GodotTask;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.LevelManagement;
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

    internal static void TriggerPlayerDataSave()
    {
        SaveTask().Forget();
        return;

        async GDTaskVoid SaveTask()
        {
            MessageBox.Print("start saving");
            await _currentPlayerSaveData.SaveAsync(OS.GetUserDataDir(), SaveFormat.Json, QuitHandler.QuitGameToken);
            MessageBox.Print("save complete successfully");
        }
    }

    internal static MainSetting MainSetting => _currentPlayerSaveData.MainSetting;
    internal static History History => _currentPlayerSaveData.History;
    internal static ActivatedCheatCodeSet ActivatedCheatCodeSet => _currentPlayerSaveData.ActivatedCheatCodeSet;
    internal static CurrentRunInfo CurrentRunInfo => _currentPlayerSaveData.CurrentRunInfo;

    // 临时更新桥（B 阶段）：在类型 record 化前，给消费端一个统一出口。
    // D 阶段会替换为 SaveData.Dispatch(s => s.UpdateX(...)) + 事件驱动保存。
    internal static void UpdateMainSetting(Func<MainSetting, MainSetting> reduce) =>
        _currentPlayerSaveData = _currentPlayerSaveData.UpdateMainSetting(reduce);

    internal static void UpdateHistory(Func<History, History> reduce) =>
        _currentPlayerSaveData = _currentPlayerSaveData.UpdateHistory(reduce);

    internal static void UpdateActivatedCheatCodeSet(Func<ActivatedCheatCodeSet, ActivatedCheatCodeSet> reduce) =>
        _currentPlayerSaveData = _currentPlayerSaveData.UpdateActivatedCheatCodeSet(reduce);

    internal static void UpdateCurrentRunInfo(Func<CurrentRunInfo, CurrentRunInfo> reduce) =>
        _currentPlayerSaveData = _currentPlayerSaveData.UpdateCurrentRunInfo(reduce);
}
