using SnekSweeper.CheatCode;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.CheatCode;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
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
        CheckButton.SetPressed(cheatCode.IsActivated);
    }

    private void OnCheckButtonPressed()
    {
        _cheatCode.IsActivated = !_cheatCode.IsActivated;
    }
}