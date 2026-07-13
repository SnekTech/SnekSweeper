using GodotGadgets.Extensions;
using GodotGadgets.UI.Pagination;
using SnekSweeper.Autoloads;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class TutorialPage : Control
{
    const int ExamplePageSize = 1;
    
    public override void _Ready()
    {
        ExampleCardContainer.ClearChildren();
        
        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var pagination = new Pagination<ExampleData>(new BuiltinExampleQuery(), ExamplePageSize);
        ExamplePagination.Init(skin, ExampleCardContainer, pagination);
    }
}