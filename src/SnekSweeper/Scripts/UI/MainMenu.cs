using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI;

[Scene]
public partial class MainMenu : CanvasLayer
{
    [Node] private Button startButton = default!;
    [Node] private Button settingsButton = default!;
    [Node] private Button historyButton = default!;

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
        settingsButton.Pressed += () => sceneManager.GotoScene(SceneName.SettingsPage);
        historyButton.Pressed += () => sceneManager.GotoScene(SceneName.HistoryPage);
    }
}