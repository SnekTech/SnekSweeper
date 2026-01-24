using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.CheatCodeSystem.UI;
using SnekSweeper.UI.History;
using SnekSweeper.UI.Settings;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI;

[SceneTree]
public partial class MainMenu : CanvasLayer
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
        Autoload.SceneSwitcher.GotoSceneAsync<HistoryPage>().Fire();
    }

    void OnCheatCodeButtonPressed()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<CheatCodePage>().Fire();
    }

    void OnSettingsButtonPressed()
    {
        Autoload.SceneSwitcher.GotoSceneAsync<SettingsPage>().Fire();
    }

    void OnStartButtonPressed()
    {
        Autoload.SceneSwitcher.LoadLevel(LoadLevelSource.CreateRegularStart(HouseKeeper.MainSetting)).Fire();
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}