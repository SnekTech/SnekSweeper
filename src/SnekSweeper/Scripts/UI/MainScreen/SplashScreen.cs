using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class SplashScreen : CenterContainer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    public void OnResolved()
    {
        PressToStartLabel.AnyKeyPressed += OnAnyKeyPressed;
    }

    public void OnExitTree()
    {
        PressToStartLabel.AnyKeyPressed -= OnAnyKeyPressed;
    }

    void OnAnyKeyPressed() => AppLogic.Input(new AppLogic.Input.AnyKeyPressed());
}