using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.Widgets;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.CheatCodeSystem.UI;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class CheatCodePage : Control, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public override void _Ready()
    {
        PopulateCheatCodeCards();
    }

    public override void _ExitTree()
    {
        SaveData.NotifySaved();
    }

    void PopulateCheatCodeCards()
    {
        foreach (var cheatCode in CheatCodeFactory.BuiltinCheatCodeList)
        {
            var card = CheatCodeCard.InstantiateOnParent(CardContainer);
            card.Init(cheatCode, _.TooltipLayer);
        }
    }
}
