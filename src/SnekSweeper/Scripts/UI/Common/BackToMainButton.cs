using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.GameStateManagement;

namespace SnekSweeper.UI.Common;

[Meta(typeof(IAutoNode))]
public partial class BackToMainButton : Button
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    public void OnResolved()
    {
        Pressed += SendBackToMainInput;
    }

    public void OnExitTree()
    {
        Pressed -= SendBackToMainInput;
    }

    void SendBackToMainInput() => AppLogic.Input(new AppState.Input.BackToMainMenu());
}