using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Autoloads;
using SnekSweeper.GameStateManagement;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.MainScreen;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class MainMenu : VBoxContainer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    AppLogic AppLogic => this.DependOn<AppLogic>();
    
    public override void _Ready()
    {
        ContinueButton.Visible = HasAnOngoingGame();
        return;

        bool HasAnOngoingGame() => HouseKeeper.CurrentRunInfo.GridSnapshot != null;
    }

    public override void _EnterTree() => RegisterEvents();
    public override void _ExitTree() => UnregisterEvents();

    void RegisterEvents()
    {
        ContinueButton.Pressed += OnContinueButtonPressed;
        StartButton.Pressed += OnStartButtonPressed;
        SettingsButton.Pressed += OnSettingsButtonPressed;
        CheatCodeButton.Pressed += OnCheatCodeButtonPressed;
        HistoryButton.Pressed += OnHistoryButtonPressed;
        QuitButton.Pressed += OnQuitPressed;
    }

    void UnregisterEvents()
    {
        ContinueButton.Pressed -= OnContinueButtonPressed;
        StartButton.Pressed -= OnStartButtonPressed;
        SettingsButton.Pressed -= OnSettingsButtonPressed;
        CheatCodeButton.Pressed -= OnCheatCodeButtonPressed;
        HistoryButton.Pressed -= OnHistoryButtonPressed;
        QuitButton.Pressed -= OnQuitPressed;
    }

    void OnHistoryButtonPressed()
    {
        AppLogic.Input(new AppLogic.Input.HistoryPressed());
    }

    void OnCheatCodeButtonPressed()
    {
        AppLogic.Input(new AppLogic.Input.CheatCodePressed());
    }

    void OnSettingsButtonPressed()
    {
        AppLogic.Input(new AppLogic.Input.SettingsPressed());
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
        // todo: implement continue
    }
}