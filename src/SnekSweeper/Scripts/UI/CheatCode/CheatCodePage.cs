using SnekSweeper.Autoloads;
using SnekSweeper.CheatCode;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.CheatCode;

[SceneTree]
public partial class CheatCodePage : Control, ISceneScript
{
    [Export] private CheatCodeCollection cheatCodeCollection = null!;

    private CheatCodeSaveData _cheatCodeSaveData = null!;

    public override void _Ready()
    {
        _cheatCodeSaveData = HouseKeeper.CheatCodeSaveData;
        PopulateCheatCodeCards();
    }

    private void PopulateCheatCodeCards()
    {
        foreach (var cheatCodeResource in cheatCodeCollection.CheatCodeResources)
        {
            var card = SceneFactory.Instantiate<CheatCodeCard>();
            var isCheatCodeActivated = _cheatCodeSaveData.IsCheatCodeActivated(cheatCodeResource.Name);
            card.Init(cheatCodeResource, OnCheatCodeToggle, isCheatCodeActivated);
            CardContainer.AddChild(card);
        }

        return;

        void OnCheatCodeToggle(string cheatCodeName, bool isButtonPressed)
        {
            _cheatCodeSaveData.SetCheatCodeStatus(cheatCodeName, isButtonPressed);
        }
    }
}