using SnekSweeper.Autoloads;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class TutorialPage : Control
{
    public override void _Ready()
    {
        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        ExamplePagination.Init(skin, ExampleCardContainer);
    }
}