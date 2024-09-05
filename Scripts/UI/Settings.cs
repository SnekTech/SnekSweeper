using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.UI;

public partial class Settings : Control
{
    [Export]
    private SkinCollection _skinCollection = null!;

    [ExportGroup("Components")]
    [Export]
    private Button _backToMainButton = null!;

    [Export]
    private OptionButton _difficultyOptionButton = null!;

    [Export]
    private OptionButton _skinOptionButton = null!;

    private MainSetting _mainSetting = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainSetting = HouseKeeper.MainSetting;
        _backToMainButton.Pressed += () => SceneManager.Instance.GotoScene(ScenePaths.MainScene);

        GenerateDifficultyOptions();
        GenerateSkinOptions();
        _difficultyOptionButton.ItemSelected += OnDifficultySelected;
        _skinOptionButton.ItemSelected += OnSkinSelected;
    }

    public override void _ExitTree()
    {
        _difficultyOptionButton.ItemSelected -= OnDifficultySelected;
        _skinOptionButton.ItemSelected -= OnSkinSelected;
    }

    private void OnDifficultySelected(long index)
    {
        _mainSetting.CurrentDifficultyIndex = (int)index;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void OnSkinSelected(long index)
    {
        _mainSetting.CurrentSkinIndex = (int)index;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void GenerateDifficultyOptions()
    {
        _difficultyOptionButton.Clear();
        for (var i = 0; i < _mainSetting.Difficulties.Length; i++)
        {
            var difficulty = _mainSetting.Difficulties[i];
            _difficultyOptionButton.AddItem(difficulty.Name, i);
        }

        _difficultyOptionButton.Select(_mainSetting.CurrentDifficultyIndex);
    }

    private void GenerateSkinOptions()
    {
        _skinOptionButton.Clear();
        var skins = _skinCollection.Skins;
        for (var i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            _skinOptionButton.AddItem(skin.Name, i);
        }

        _skinOptionButton.Select(_mainSetting.CurrentSkinIndex);
    }
}