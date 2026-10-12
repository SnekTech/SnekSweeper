using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.AppFlow;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class SplashScreen : CenterContainer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    IAppSessionHost AppSessionHost => this.DependOn<IAppSessionHost>();

    public void OnResolved()
    {
        _.PressToStartLabel.Text = CoreFS.Say.hello("F# in GD");
        PressToStartLabel.AnyKeyPressed += OnAnyKeyPressed;
    }

    public void OnExitTree()
    {
        PressToStartLabel.AnyKeyPressed -= OnAnyKeyPressed;
    }

    void OnAnyKeyPressed() => AppSessionHost.Send(new AppSession.Event.AnyKeyPressed());
}