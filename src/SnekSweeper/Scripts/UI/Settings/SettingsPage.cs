using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Settings;

[SceneTree]
public partial class SettingsPage : CanvasLayer, ISceneScript
{
    public override void _ExitTree()
    {
        HouseKeeper.TriggerPlayerDataSave();
    }
}