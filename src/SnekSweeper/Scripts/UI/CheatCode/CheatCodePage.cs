using Godot;
using GodotUtilities;
using SnekSweeper.CheatCode;

namespace SnekSweeper.UI.CheatCode;

[Scene]
public partial class CheatCodePage : Control
{
    [Export] private PackedScene cardScene = null!;
    [Export] private CheatCodeCollection cheatCodeCollection = null!;

    [Node] private GridContainer cardContainer = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        PopulateCheatCodeCards();
    }

    private void PopulateCheatCodeCards()
    {
        foreach (var cheatCodeResource in cheatCodeCollection.CheatCodeResources)
        {
            var card = cardScene.Instantiate<CheatCodeCard>();
            card.SetContent(cheatCodeResource);
            cardContainer.AddChild(card);
        }
    }
}