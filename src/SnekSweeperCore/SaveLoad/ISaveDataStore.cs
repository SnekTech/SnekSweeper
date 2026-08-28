using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.SaveLoad;

/// <summary>
/// 存档状态存储的抽象：只读 <see cref="State"/> + 唯一写入入口 <see cref="Dispatch"/>，
/// 以及"保存被确认"的信号 <see cref="NotifySaved"/>。
/// Godot 层 <c>SaveData</c> 实现；Core 层（如 <c>GameRunRecorder</c>）与各场景依赖它保持解耦。
/// </summary>
public interface ISaveDataStore
{
    PlayerSaveData State { get; }
    void Dispatch(Func<PlayerSaveData, PlayerSaveData> reduce);
    event Action? SavedFeedback;
    void NotifySaved();
}

/// <summary>
/// <see cref="ISaveDataStore"/> 的便捷访问（与接口共址）：领域级 UpdateX 糖 + 派生查询（如 <see cref="CurrentSkin"/>）。
/// 读 State、写走 Dispatch + Core lens。
/// </summary>
public static class SaveDataStoreExtensions
{
    extension(ISaveDataStore store)
    {
        public GridSkin CurrentSkin => store.State.MainSetting.CurrentSkinKey.ToSkin();

        public void UpdateMainSetting(Func<MainSetting, MainSetting> reduce) =>
            store.Dispatch(s => s.UpdateMainSetting(reduce));

        public void UpdateHistory(Func<History, History> reduce) =>
            store.Dispatch(s => s.UpdateHistory(reduce));

        public void UpdateActivatedCheatCodeSet(Func<ActivatedCheatCodeSet, ActivatedCheatCodeSet> reduce) =>
            store.Dispatch(s => s.UpdateActivatedCheatCodeSet(reduce));

        public void UpdateCurrentRunInfo(Func<CurrentRunInfo, CurrentRunInfo> reduce) =>
            store.Dispatch(s => s.UpdateCurrentRunInfo(reduce));
    }
}
