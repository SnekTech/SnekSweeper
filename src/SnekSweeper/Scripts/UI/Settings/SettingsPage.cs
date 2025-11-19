using SnekSweeper.Autoloads;
using SnekSweeper.GameSettings;
using SnekSweeper.GridSystem;
using SnekSweeper.SaveLoad;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class SettingsPage : CanvasLayer, ISceneScript
{
    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    #region lifecycle

    public override void _EnterTree()
    {
        DifficultyOptionButton.ItemSelected += OnDifficultySelected;
        ComboRankDisplayToggle.Pressed += OnComboRankDisplayToggled;
    }

    public override void _Ready()
    {
        InitDifficultyOptions();
        InitComboRankDisplayToggle();
    }

    public override void _ExitTree()
    {
        DifficultyOptionButton.ItemSelected -= OnDifficultySelected;
        ComboRankDisplayToggle.Pressed -= OnComboRankDisplayToggled;
    }

    #endregion

    private void OnDifficultySelected(long index)
    {
        _mainSetting.CurrentDifficulty = DifficultyFactory.GetDifficultyById(DifficultyOptionButton.GetSelectedId());
        SaveLoadEventBus.EmitSaveRequested();
    }


    private void OnComboRankDisplayToggled()
    {
        _mainSetting.ComboRankDisplay = !_mainSetting.ComboRankDisplay;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void InitDifficultyOptions()
    {
        DifficultyOptionButton.Clear();
        var difficulties = DifficultyFactory.Difficulties;
        foreach (var difficulty in difficulties)
        {
            DifficultyOptionButton.AddItem(difficulty.Name, difficulty.Id);
        }

        var savedDifficultyIndex =
            difficulties.FindIndex(difficulty => difficulty.Id == _mainSetting.CurrentDifficulty.Id);
        DifficultyOptionButton.Select(savedDifficultyIndex);
    }


    private void InitComboRankDisplayToggle()
    {
        ComboRankDisplayToggle.SetPressedNoSignal(_mainSetting.ComboRankDisplay);
    }
}