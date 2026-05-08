using GodotGadgets.Extensions;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.CheatCodeSystem.UI;
using SnekSweeper.UI.History;
using SnekSweeper.UI.Settings;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.MainScreen;

[SceneTree]
public partial class MainMenu : VBoxContainer
{
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
        Autoload.SceneSwitcher.GotoSceneAsync<HistoryPage>().Forget();
    }

    void OnCheatCodeButtonPressed()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<CheatCodePage>().Forget();
    }

    void OnSettingsButtonPressed()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<SettingsPage>().Forget();
    }

    void OnStartButtonPressed()
    {
        Autoload.SceneSwitcher.LoadLevel(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting)).Forget();
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }

    void OnContinueButtonPressed()
    {
        "continue pressed, ongoing grid snapshot:".DumpGd();
        HouseKeeper.CurrentRunInfo.GridSnapshot!.SnapshotStates.DumpGd();
    }
}