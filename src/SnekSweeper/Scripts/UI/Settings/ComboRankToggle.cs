using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.Settings;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class ComboRankToggle : HBoxContainer
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _EnterTree()
    {
        ComboRankDisplayToggle.Toggled += OnComboRankDisplayToggled;
    }

    public override void _ExitTree()
    {
        ComboRankDisplayToggle.Toggled -= OnComboRankDisplayToggled;
    }

    public void OnResolved()
    {
        ComboRankDisplayToggle.SetPressedNoSignal(SaveData.State.MainSetting.ComboRankDisplay);
    }

    void OnComboRankDisplayToggled(bool toggledOn)
    {
        SaveData.UpdateMainSetting(m => m with { ComboRankDisplay = toggledOn });
    }
}
