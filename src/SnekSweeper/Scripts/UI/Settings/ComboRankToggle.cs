using SnekSweeper.Autoloads;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class ComboRankToggle : HBoxContainer
{
    public override void _Ready()
    {
        InitComboRankDisplayToggle();
    }

    public override void _EnterTree()
    {
        ComboRankDisplayToggle.Toggled += OnComboRankDisplayToggled;
    }

    public override void _ExitTree()
    {
        ComboRankDisplayToggle.Toggled -= OnComboRankDisplayToggled;
    }

    void InitComboRankDisplayToggle()
    {
        ComboRankDisplayToggle.SetPressedNoSignal(HouseKeeper.MainSetting.ComboRankDisplay);
    }

    static void OnComboRankDisplayToggled(bool toggledOn)
    {
        HouseKeeper.MainSetting.ComboRankDisplay = toggledOn;
        HouseKeeper.SaveCurrentPlayerData();
    }
}