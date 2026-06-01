using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.GameStateManagement;

namespace SnekSweeper;

[Meta(typeof(IAutoNode))]
public partial class Main : Node, IProvide<AppLogic>, IProvide<IAppRepo>
{
    [Node]
    public ISceneSwitcher SceneSwitcher { get; set; } = null!;

    IAppRepo AppRepo { get; set; } = null!;
    AppLogic _appLogic = null!;
    AppLogic IProvide<AppLogic>.Value() => _appLogic;
    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

    public void OnReady()
    {
        AppRepo = new AppRepo();
        _appLogic = new AppLogic();
        _appLogic.Set(SceneSwitcher);
        _appLogic.Set(new AppLogic.Data());
        _appLogic.Set(AppRepo);
        this.Provide();

        _appLogic.Start();
    }

    public override void _Notification(int what) => this.Notify(what);
}