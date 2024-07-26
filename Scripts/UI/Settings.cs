using Godot;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;
using SnekSweeper.GameSettings;

namespace SnekSweeper.UI;

public partial class Settings : Control
{
    [Export]
    private MainSetting _mainSetting;

    private OptionButton _optionButton;
    private Button _backToMainButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _optionButton = GetNode<OptionButton>("%OptionButton");
        _backToMainButton = GetNode<Button>("%BackToMainButton");

        _optionButton.ItemSelected += OptionButtonOnItemSelected;

        _backToMainButton.Pressed += () => SceneManager.Instance.GotoScene(ScenePaths.MainScene);
    }

    private void OptionButtonOnItemSelected(long index)
    {
        _mainSetting.SetDifficulty((int)index);
    }
}