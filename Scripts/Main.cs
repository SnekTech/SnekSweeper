using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper;

[Scene]
public partial class Main : Node
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
        var sceneManager = SceneManager.Instance;
        startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
        settingsButton.Pressed += () => sceneManager.GotoScene(ScenePaths.SettingsScene);
        historyButton.Pressed += () => sceneManager.GotoScene(ScenePaths.HistoryScene);
    }
}