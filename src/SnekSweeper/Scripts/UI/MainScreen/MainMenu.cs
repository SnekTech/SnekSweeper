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
    public override void _EnterTree() => RegisterEvents();
    public override void _ExitTree() => UnregisterEvents();

    void RegisterEvents()
    {
        StartButton.Pressed += OnStartButtonPressed;
        SettingsButton.Pressed += OnSettingsButtonPressed;
        CheatCodeButton.Pressed += OnCheatCodeButtonPressed;
        HistoryButton.Pressed += OnHistoryButtonPressed;
        QuitButton.Pressed += OnQuitPressed;
    }

    void UnregisterEvents()
    {
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
}