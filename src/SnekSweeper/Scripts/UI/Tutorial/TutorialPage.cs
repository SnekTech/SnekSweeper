using GodotGadgets.Extensions;
using GodotGadgets.UI.Pagination;
using SnekSweeper.Autoloads;
using SnekSweeper.UI.Tutorial.Example;
using SnekSweeper.Widgets;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

[SceneTree(root: "ROOT")]
public partial class TutorialPage : Control
{
    const int ExamplePageSize = 1;

    public override void _Ready()
    {
        ExampleCardContainer.ClearChildren();

        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var pagination = new Pagination<ExampleData>(new BuiltinExampleQuery(), ExamplePageSize);
        ExamplePaginationBar.Bind(
            ExampleCardContainer,
            _ =>
            {
                var card = ExampleCard.Instantiate();
                card.Skin = skin;
                return card;
            }
            , pagination);
    }
}