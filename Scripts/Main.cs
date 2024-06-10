using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper;

public partial class Main : Node
{
    public override void _Ready()
    {
        var sceneManager = GetNode<SceneManager>($"/root/{nameof(SceneManager)}");
        var startButton = GetNode<Button>("StartButton");
        startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
    }
}