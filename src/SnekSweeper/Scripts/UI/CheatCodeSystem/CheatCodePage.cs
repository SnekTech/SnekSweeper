using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.CheatCodeSystem;

[SceneTree]
public partial class CheatCodePage : Control, ISceneScript
{
    public override void _Ready()
    {
        PopulateCheatCodeCards();
    }

    private void PopulateCheatCodeCards()
    {
        foreach (var cheatCode in CheatCodeFactory.BuiltinCheatCodeList)
        {
            var card = SceneFactory.Instantiate<CheatCodeCard>();
            card.Init(cheatCode);
            CardContainer.AddChild(card);
        }
    }
}