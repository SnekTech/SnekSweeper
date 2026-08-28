using GodotGadgets.Extensions;
using GodotTask;
using SnekGameDevKit;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.Autoloads;

/// <summary>
/// Single source of truth for the player's save data (D 阶段)。
/// 唯一写入入口 <see cref="Dispatch"/>；每次状态变更入队落盘并触发 <see cref="StateChanged"/>。
/// 落盘由 <see cref="SaveQueue{T}"/> 单写者串行执行：写顺序 = Dispatch 顺序，突发合并只写最新。
/// 当前用 static 访问器过渡，E 阶段改为 DI 注入（Get&lt;SaveData&gt;()）。
/// </summary>
public partial class SaveData : Node, ISaveDataStore
{
    public static SaveData Instance { get; private set; } = null!;

    PlayerSaveData _state = null!;
    readonly SaveQueue<PlayerSaveData> _saveQueue =
        new((state, ct) => state.SaveAsync(OS.GetUserDataDir(), SaveFormat.Json, ct));
    public PlayerSaveData State => _state;
    public event Action<PlayerSaveData>? StateChanged;
    public event Action? SavedFeedback;

    public override void _Ready()
    {
        Instance = this;

        _state = LoadOrCreate();
        _saveQueue.RunAsync(QuitHandler.QuitGameToken).AsGDTask().Forget();
    }

    public void Dispatch(Func<PlayerSaveData, PlayerSaveData> reduce)
    {
        var next = reduce(_state);
        if (ReferenceEquals(next, _state)) return;

        _state = next;
        StateChanged?.Invoke(_state);
        _saveQueue.RequestSave(_state);
    }

    public static void SaveNow() => Instance._state.Save(OS.GetUserDataDir());

    /// <summary>用户可感知的保存时刻反馈（落盘已自动完成，这里只发事件，由显示层决定 toast）。</summary>
    public static void NotifySaved() => Instance.SavedFeedback?.Invoke();

    // ---- 领域访问器（static 过渡；E 阶段换 DI 实例）----
    public static MainSetting MainSetting => Instance.State.MainSetting;
    public static History History => Instance.State.History;
    public static ActivatedCheatCodeSet ActivatedCheatCodeSet => Instance.State.ActivatedCheatCodeSet;
    public static CurrentRunInfo CurrentRunInfo => Instance.State.CurrentRunInfo;

    public static void UpdateMainSetting(Func<MainSetting, MainSetting> reduce) =>
        Instance.Dispatch(s => s.UpdateMainSetting(reduce));

    public static void UpdateHistory(Func<History, History> reduce) =>
        Instance.Dispatch(s => s.UpdateHistory(reduce));

    public static void UpdateActivatedCheatCodeSet(Func<ActivatedCheatCodeSet, ActivatedCheatCodeSet> reduce) =>
        Instance.Dispatch(s => s.UpdateActivatedCheatCodeSet(reduce));

    public static void UpdateCurrentRunInfo(Func<CurrentRunInfo, CurrentRunInfo> reduce) =>
        Instance.Dispatch(s => s.UpdateCurrentRunInfo(reduce));

    static PlayerSaveData LoadOrCreate()
    {
        var loaded = PlayerSaveData.Load(OS.GetUserDataDir());
        if (loaded is not null) return loaded;

        "cannot load player save data from disk, will create an empty new one".DumpGd();
        return PlayerSaveData.CreateEmpty();
    }
}
