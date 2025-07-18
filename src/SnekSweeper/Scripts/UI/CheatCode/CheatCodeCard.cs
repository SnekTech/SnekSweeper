using SnekSweeper.Autoloads;
using SnekSweeper.CheatCode;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.CheatCode;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    private readonly ActivatedCheatCodeSet activatedSet = HouseKeeper.ActivatedCheatCodeSet;

    private CheatCodeData _cheatCode = null!;

    public override void _EnterTree()
    {
        CheckButton.Pressed += OnCheckButtonPressed;
    }

    public override void _ExitTree()
    {
        CheckButton.Pressed -= OnCheckButtonPressed;
    }

    public void Init(CheatCodeData cheatCode)
    {
        _cheatCode = cheatCode;

        NameLabel.Text = cheatCode.Name;
        Icon.Texture = cheatCode.Icon;
        CheckButton.SetPressed(activatedSet.Contains(cheatCode));
    }

    private void OnCheckButtonPressed()
    {
        if (activatedSet.Contains(_cheatCode))
        {
            activatedSet.Remove(_cheatCode);
        }
        else
        {
            activatedSet.Add(_cheatCode);
        }
    }
}