using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper;

[Meta(typeof(IAutoNode))]
public partial class Main : Node, IProvide<AppLogic>, IProvide<IAppRepo>, IProvide<ISaveDataStore>
{
    [Node]
    public ISceneSwitcher SceneSwitcher { get; set; } = null!;

    IAppRepo AppRepo { get; set; } = null!;
    AppLogic _appLogic = null!;
    AppLogic IProvide<AppLogic>.Value() => _appLogic;
    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;
    ISaveDataStore IProvide<ISaveDataStore>.Value() => SaveData.Instance;

    public void OnReady()
    {
        AppRepo = new AppRepo();
        _appLogic = new AppLogic();
        _appLogic.Set(SceneSwitcher);
        _appLogic.Set(AppRepo);
        this.Provide();

        SaveData.Instance.SavedFeedback += () => MessageBox.Print("已保存");

        _appLogic.Start<AppState.SplashScreen>();
    }

    public override void _Notification(int what) => this.Notify(what);
}
