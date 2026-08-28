using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Widgets;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.Settings;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class SettingsPage : CanvasLayer, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _ExitTree()
    {
        SaveData.NotifySaved();
    }
}
