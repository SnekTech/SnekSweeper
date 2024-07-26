using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper;

public partial class Main : Node
{
    public override void _Ready()
    {
        var sceneManager = GetNode<SceneManager>($"/root/{nameof(SceneManager)}");
        var startButton = GetNode<Button>("%StartButton");
        var settingsButton = GetNode<Button>("%SettingsButton");
        
        startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
        settingsButton.Pressed += () => sceneManager.GotoScene(ScenePaths.SettingsScene);
    }
}