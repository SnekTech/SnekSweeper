using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper;

public partial class Main : Node
{
    [Export]
    private Button _startButton = null!;
    [Export]
    private Button _settingsButton = null!;

    public override void _Ready()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneManager = SceneManager.Instance;
        _startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
        _settingsButton.Pressed += () => sceneManager.GotoScene(ScenePaths.SettingsScene);
    }
}