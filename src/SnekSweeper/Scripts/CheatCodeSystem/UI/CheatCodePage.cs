using SnekSweeper.Widgets;

namespace SnekSweeper.CheatCodeSystem.UI;

[SceneTree]
public partial class CheatCodePage : Control, ISceneScript
{
    public override void _Ready()
    {
        PopulateCheatCodeCards();
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