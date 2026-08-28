using GodotGadgets.Extensions;
using GodotTask;
using SnekGameDevKit;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.Autoloads;

/// <summary>
/// Single source of truth for the player's save data。
/// 唯一写入入口 <see cref="Dispatch"/>；每次状态变更入队落盘并触发 <see cref="StateChanged"/>。
/// 落盘由 <see cref="SaveQueue{T}"/> 单写者串行执行：写顺序 = Dispatch 顺序，突发合并只写最新。
/// 作为 <see cref="ISaveDataStore"/> 注入给各场景（E 阶段）；便捷访问见 SaveDataStoreExtensions。
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

    public void SaveNow() => _state.Save(OS.GetUserDataDir());

    /// <summary>用户可感知的保存时刻反馈（落盘已自动完成，这里只发事件，由显示层决定 toast）。</summary>
    public void NotifySaved() => SavedFeedback?.Invoke();

    static PlayerSaveData LoadOrCreate()
    {
        var loaded = PlayerSaveData.Load(OS.GetUserDataDir());
        if (loaded is not null) return loaded;

        "cannot load player save data from disk, will create an empty new one".DumpGd();
        return PlayerSaveData.CreateEmpty();
    }
}
