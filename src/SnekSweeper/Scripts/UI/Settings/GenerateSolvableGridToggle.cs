using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.Settings;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class GenerateSolvableGridToggle : HBoxContainer
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _EnterTree()
    {
        Toggle.Toggled += OnSolvableToggled;
    }

    public override void _ExitTree()
    {
        Toggle.Toggled -= OnSolvableToggled;
    }

    public void OnResolved()
    {
        Toggle.SetPressedNoSignal(SaveData.State.MainSetting.GenerateSolvableGrid);
    }

    void OnSolvableToggled(bool toggledOn)
    {
        SaveData.UpdateMainSetting(m => m with { GenerateSolvableGrid = toggledOn });
    }
}
