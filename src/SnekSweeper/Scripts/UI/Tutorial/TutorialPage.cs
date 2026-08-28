using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.Extensions;
using GodotGadgets.UI.Pagination;
using SnekSweeper.UI.Tutorial.Example;
using SnekSweeper.Widgets;
using SnekSweeperCore.SaveLoad;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

[Meta(typeof(IAutoNode))]
[SceneTree(root: "ROOT")]
public partial class TutorialPage : Control, ISceneScript
{
    const int ExamplePageSize = 1;
    
    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _Ready()
    {
        ExampleCardContainer.ClearChildren();
    }

    public void OnResolved()
    {
        var pagination = new Pagination<ExampleData>(new BuiltinExampleQuery(), ExamplePageSize);
        ExamplePaginationBar.Bind(
            ExampleCardContainer,
            _ =>
            {
                var card = ExampleCard.Instantiate();
                card.Skin = SaveData.CurrentSkin;
                return card;
            }
            , pagination);
    }

    public override void _Notification(int what) => this.Notify(what);
}
