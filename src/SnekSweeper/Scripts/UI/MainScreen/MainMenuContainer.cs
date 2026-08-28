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
        if (HasAnOngoingGame())
        {
            bindings.Insert(0, CreateButtonWithBinding("Continue", OnContinueButtonPressed));
        }

        _.ScrollMenuView.Init(bindings);
        return;

        bool HasAnOngoingGame() => SaveData.State.CurrentRunInfo.GridSnapshot != null;
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

    void OnContinueButtonPressed()
    {
        // todo: refactor this ! operator
        var fromSnapshot =
            new FromGridSnapshot(SaveData.State.CurrentRunInfo.GridSnapshot!, SaveData.State.CurrentRunInfo.StartInfo);
        AppLogic.InputNewGame(fromSnapshot);
    }

    static MenuItemBinding CreateButtonWithBinding(string buttonText, Action onConfirm) =>
        new(new MenuItemButton { Text = buttonText }, onConfirm);
}
