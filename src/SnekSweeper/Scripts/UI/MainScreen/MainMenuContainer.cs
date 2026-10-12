using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.UI.ScrollMenuCore;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.AppFlow;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class MainMenuContainer : Control, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    IAppSessionHost AppSessionHost => this.DependOn<IAppSessionHost>();

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public void OnResolved()
    {
        var bindings = new List<MenuItemBinding>
        {
            CreateButtonWithBinding("Start", OnStartButtonPressed),
            CreateButtonWithBinding("Settings", OnSettingsButtonPressed),
            CreateButtonWithBinding("CheatCode", OnCheatCodeButtonPressed),
            CreateButtonWithBinding("History", OnHistoryButtonPressed),
            CreateButtonWithBinding("Tutorial", OnTutorialButonPressed),
            CreateButtonWithBinding("Quit", OnQuitPressed),
        };
        TrySetupContinueButton();

        _.ScrollMenuView.Init(bindings);
        return;

        void TrySetupContinueButton()
        {
            if (SaveData.State.CurrentRunInfo.OngoingGame is not { } ongoingGame) return;

            bindings.Insert(0, CreateButtonWithBinding("Continue", () =>
            {
                var fromOngoingGame = new FromOngoingGame(ongoingGame);
                AppSessionHost.Send(new AppSession.Event.NewGame(fromOngoingGame));
            }));
        }
    }

    void OnTutorialButonPressed()
    {
        AppSessionHost.Send(new AppSession.Event.TutorialPressed());
    }

    void OnHistoryButtonPressed()
    {
        AppSessionHost.Send(new AppSession.Event.HistoryPressed());
    }

    void OnCheatCodeButtonPressed()
    {
        AppSessionHost.Send(new AppSession.Event.CheatCodePressed());
    }

    void OnSettingsButtonPressed()
    {
        AppSessionHost.Send(new AppSession.Event.SettingsPressed());
    }

    void OnStartButtonPressed()
    {
        var newGameSource = LoadLevelSource.CreateRegularStart(SaveData.State.MainSetting);
        AppSessionHost.Send(new AppSession.Event.NewGame(newGameSource));
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }

    static MenuItemBinding CreateButtonWithBinding(string buttonText, Action onConfirm) =>
        new(new MenuItemButton { Text = buttonText }, onConfirm);
}