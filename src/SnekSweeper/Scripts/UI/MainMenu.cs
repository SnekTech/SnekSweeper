using SnekSweeper.CheatCodeSystem.UI;
using SnekSweeper.Levels;
using SnekSweeper.UI.History;
using SnekSweeper.UI.Settings;

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
        Autoload.SceneSwitcher.GotoScene<HistoryPage>();
    }

    void OnCheatCodeButtonPressed()
    {
        Autoload.SceneSwitcher.GotoScene<CheatCodePage>();
    }

    void OnSettingsButtonPressed()
    {
        Autoload.SceneSwitcher.GotoScene<SettingsPage>();
    }


    void OnStartButtonPressed()
    {
        Autoload.SceneSwitcher.GotoScene<Level1>();
    }

    void OnQuitPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}