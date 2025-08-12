using SnekSweeper.Widgets;

namespace SnekSweeper.CheatCodeSystem.UI;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    private CheatCode _cheatCode = null!;

    public override void _EnterTree()
    {
        CheckButton.Pressed += OnCheckButtonPressed;
    }

    public override void _ExitTree()
    {
        CheckButton.Pressed -= OnCheckButtonPressed;
    }

    public void Init(CheatCode cheatCode)
    {
        _cheatCode = cheatCode;

        NameLabel.Text = cheatCode.Data.Name;
        Icon.Texture = cheatCode.Icon;
        CheckButton.SetPressed(cheatCode.IsActivated);
    }

    private void OnCheckButtonPressed()
    {
        _cheatCode.IsActivated = !_cheatCode.IsActivated;
    }
}