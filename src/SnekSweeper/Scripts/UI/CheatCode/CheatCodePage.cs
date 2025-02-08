using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.CheatCode;

namespace SnekSweeper.UI.CheatCode;

[Scene]
public partial class CheatCodePage : Control
{
    [Export] private PackedScene cardScene = null!;
    [Export] private CheatCodeCollection cheatCodeCollection = null!;

    [Node] private GridContainer cardContainer = null!;

    private CheatCodeSaveData _cheatCodeSaveData = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        _cheatCodeSaveData = HouseKeeper.CheatCodeSaveData;
        PopulateCheatCodeCards();
    }

    private void PopulateCheatCodeCards()
    {
        foreach (var cheatCodeResource in cheatCodeCollection.CheatCodeResources)
        {
            var card = cardScene.Instantiate<CheatCodeCard>();
            var isCheatCodeActivated = _cheatCodeSaveData.IsCheatCodeActivated(cheatCodeResource.Name);
            card.Init(cheatCodeResource, OnCheatCodeToggle, isCheatCodeActivated);
            cardContainer.AddChild(card);
        }

        return;

        void OnCheatCodeToggle(string cheatCodeName, bool isButtonPressed)
        {
            _cheatCodeSaveData.SetCheatCodeStatus(cheatCodeName, isButtonPressed);
        }
    }
}