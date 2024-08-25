using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.SaveLoad;

namespace SnekSweeper;

public partial class Main : Node
{
    private PlayerData _playerData = null!;
    private Button _startButton = null!;
    private Button _settingsButton = null!;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("%StartButton");
        _settingsButton = GetNode<Button>("%SettingsButton");

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneManager = SceneManager.Instance;
        _startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
        _settingsButton.Pressed += () => sceneManager.GotoScene(ScenePaths.SettingsScene);
    }
}