using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper;

public partial class Main : Node
{
    [Export]
    private MainSetting _mainSetting = null!;

    private PlayerData _playerData = null!;
    private Button _startButton = null!;
    private Button _settingsButton = null!;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("%StartButton");
        _settingsButton = GetNode<Button>("%SettingsButton");

        RegisterEvents();

        CreateOrLoadPlayerSave();
    }

    public override void _ExitTree()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        var sceneManager = SceneManager.Instance;
        _startButton.Pressed += () => sceneManager.GotoScene(ScenePaths.Level1);
        _settingsButton.Pressed += () => sceneManager.GotoScene(ScenePaths.SettingsScene);

        SaveLoadEventBus.SaveRequested += SavePlayerData;
    }

    private void UnregisterEvents()
    {
        SaveLoadEventBus.SaveRequested -= SavePlayerData;
    }

    private void CreateOrLoadPlayerSave()
    {
        if (PlayerData.Exists)
        {
            _playerData = PlayerData.Load();
        }
        else
        {
            _playerData = new PlayerData();
            _playerData.CurrentDifficultyIndex = 0;
            _playerData.Save();
        }

        _mainSetting.SetDifficulty(_playerData.CurrentDifficultyIndex);
    }

    private void SavePlayerData()
    {
        _playerData.Save();
    }
}