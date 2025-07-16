using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.UI.Settings;

namespace SnekSweeper.UI;

[Scene]
public partial class MainMenu : CanvasLayer
{
    [Node] private Button startButton = null!;
    [Node] private Button settingsButton = null!;
    [Node] private Button historyButton = null!;
    [Node] private Button quitButton = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneManager = SceneSwitcher.Instance;
        startButton.Pressed += () => sceneManager.GotoScene(SceneName.Level1);
        settingsButton.Pressed += () => sceneManager.GotoScene<SettingsPage>();
        historyButton.Pressed += () => sceneManager.GotoScene(SceneName.HistoryPage);

        quitButton.Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}