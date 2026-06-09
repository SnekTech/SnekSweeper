using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.UI.Tutorial.Example;
using SnekSweeper.Widgets;
using SnekSweeperCore.SkinSystem;
using SnekSweeperCore.Tutorial;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class TutorialPage : Control
{
    public override void _Ready()
    {
        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        TriggerPopulateExamples(skin).Forget();
    }
    
    async GDTaskVoid TriggerPopulateExamples(GridSkin skin)
    {
        var examples = TutorialExampleCollection.BuiltinExamples;
        
        var initTasks = new List<GDTask>();
        
        foreach (var example in examples)
        {
            var card = ExampleCard.InstantiateOnParent(ExampleCardContainer);
            initTasks.Add(card.InitAsync(example, skin, this.GetCancellationTokenOnTreeExit()));
        }

        await GDTask.WhenAll(initTasks);
    }
}