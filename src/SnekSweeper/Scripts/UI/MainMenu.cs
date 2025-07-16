using SnekSweeper.Autoloads;
using SnekSweeper.UI.History;
using SnekSweeper.UI.Settings;

namespace SnekSweeper.UI;

[SceneTree]
public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneManager = SceneSwitcher.Instance;
        StartButton.Pressed += () => sceneManager.GotoScene<Levels.Level1>();
        SettingsButton.Pressed += () => sceneManager.GotoScene<SettingsPage>();
        HistoryButton.Pressed += () => sceneManager.GotoScene<HistoryPage>();

        QuitButton.Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}