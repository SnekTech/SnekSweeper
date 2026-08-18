using SnekSweeper.Autoloads;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class GenerateSolvableGridToggle : HBoxContainer
{
    public override void _Ready()
    {
        InitSolvableToggle();
    }

    public override void _EnterTree()
    {
        Toggle.Toggled += OnSolvableToggled;
    }

    public override void _ExitTree()
    {
        Toggle.Toggled -= OnSolvableToggled;
    }

    void InitSolvableToggle()
    {
        Toggle.SetPressedNoSignal(SaveData.MainSetting.GenerateSolvableGrid);
    }

    static void OnSolvableToggled(bool toggledOn)
    {
        SaveData.UpdateMainSetting(m => m with { GenerateSolvableGrid = toggledOn });
    }
}
