using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.GameSettings;
using SnekSweeper.SaveLoad;

namespace SnekSweeper.UI;

public partial class Settings : Control
{
    private OptionButton _optionButton = null!;
    private Button _backToMainButton = null!;
    private MainSetting _mainSetting = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainSetting = HouseKeeper.MainSetting;
        
        _optionButton = GetNode<OptionButton>("%OptionButton");
        _backToMainButton = GetNode<Button>("%BackToMainButton");
        
        GenerateDifficultyOptions();

        _optionButton.ItemSelected += OptionButtonOnItemSelected;
        _backToMainButton.Pressed += () => SceneManager.Instance.GotoScene(ScenePaths.MainScene);
    }

    private void OptionButtonOnItemSelected(long index)
    {
        _mainSetting.CurrentDifficultyIndex = (int)index;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void GenerateDifficultyOptions()
    {
        _optionButton.Clear();
        foreach (var difficulty in _mainSetting.Difficulties)
        {
            _optionButton.AddItem(difficulty.Name);
        }
        
        _optionButton.Select(_mainSetting.CurrentDifficultyIndex);
    }
}