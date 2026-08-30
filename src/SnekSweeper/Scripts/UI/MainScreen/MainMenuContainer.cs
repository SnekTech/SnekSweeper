using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.UI.ScrollMenuCore;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class MainMenuContainer : Control, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

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

            var fromOngoingGame = new FromOngoingGame(ongoingGame);
            bindings.Insert(0, CreateButtonWithBinding("Continue", () => AppLogic.InputNewGame(fromOngoingGame)));
        }
    }

    void OnTutorialButonPressed()
    {
        AppLogic.Input(new AppState.Input.TutorialPressed());
    }

    void OnHistoryButtonPressed()
    {
        AppLogic.Input(new AppState.Input.HistoryPressed());
    }

    void OnCheatCodeButtonPressed()
    {
        AppLogic.Input(new AppState.Input.CheatCodePressed());
    }

    void OnSettingsButtonPressed()
    {
        AppLogic.Input(new AppState.Input.SettingsPressed());
    }

    void OnStartButtonPressed()
    {
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(SaveData.State.MainSetting));
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }

    static MenuItemBinding CreateButtonWithBinding(string buttonText, Action onConfirm) =>
        new(new MenuItemButton { Text = buttonText }, onConfirm);
}
