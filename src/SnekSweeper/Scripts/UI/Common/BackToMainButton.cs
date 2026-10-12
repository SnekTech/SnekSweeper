using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.GameStateManagement;
using SnekSweeperCore.AppFlow;

namespace SnekSweeper.UI.Common;

[Meta(typeof(IAutoNode))]
public partial class BackToMainButton : Button
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    IAppSessionHost AppSessionHost => this.DependOn<IAppSessionHost>();

    public void OnResolved()
    {
        Pressed += SendBackToMainInput;
    }

    public void OnExitTree()
    {
        Pressed -= SendBackToMainInput;
    }

    void SendBackToMainInput() => AppSessionHost.Send(new AppSession.Event.BackToMainMenu());
}