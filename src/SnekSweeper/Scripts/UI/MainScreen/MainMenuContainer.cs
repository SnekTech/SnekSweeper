using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.UI.ScrollMenuCore;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class MainMenuContainer : Control, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();

    public override void _Ready()
    {
        var bindings = new List<MenuItemBinding>
        {
            CreateButtonWithBinding("Start", OnStartButtonPressed),
            CreateButtonWithBinding("Settings", OnSettingsButtonPressed),
            CreateButtonWithBinding("CheatCode", OnCheatCodeButtonPressed),
            CreateButtonWithBinding("History", OnHistoryButtonPressed),
            CreateButtonWithBinding("Quit", OnQuitPressed),
        };
        if (HasAnOngoingGame())
        {
            bindings.Insert(0, CreateButtonWithBinding("Continue", OnContinueButtonPressed));
        }

        _.ScrollMenuView.Init(bindings);
        return;

        bool HasAnOngoingGame() => HouseKeeper.CurrentRunInfo.GridSnapshot != null;
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
        AppLogic.InputNewGame(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting));
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }

    void OnContinueButtonPressed()
    {
        var fromSnapshot =
            new FromGridSnapshot(HouseKeeper.CurrentRunInfo.GridSnapshot!, HouseKeeper.CurrentRunInfo.StartInfo);
        AppLogic.InputNewGame(fromSnapshot);
    }

    static MenuItemBinding CreateButtonWithBinding(string buttonText, Action onConfirm) =>
        new(new MenuItemButton { Text = buttonText }, onConfirm);
}