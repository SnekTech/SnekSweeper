using SnekSweeper.Autoloads;
using SnekSweeper.GameSettings;
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
        ComboRankDisplayToggle.Pressed += OnComboRankDisplayToggled;
    }

    public override void _Ready()
    {
        InitComboRankDisplayToggle();
    }

    public override void _ExitTree()
    {
        ComboRankDisplayToggle.Pressed -= OnComboRankDisplayToggled;
    }

    #endregion

    private void OnComboRankDisplayToggled()
    {
        _mainSetting.ComboRankDisplay = !_mainSetting.ComboRankDisplay;
        SaveLoadEventBus.EmitSaveRequested();
    }

    private void InitComboRankDisplayToggle()
    {
        ComboRankDisplayToggle.SetPressedNoSignal(_mainSetting.ComboRankDisplay);
    }
}