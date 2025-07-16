using SnekSweeper.CheatCode;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.CheatCode;

[SceneTree]
public partial class CheatCodeCard : PanelContainer, ISceneScript
{
    public delegate void CheatCodeToggleHandler(string cheatCodeName, bool isButtonChecked);

    private CheatCodeToggleHandler _onToggle = null!;
    private CheatCodeResource _cheatCodeResource = null!;

    public override void _EnterTree()
    {
        CheckButton.Pressed += OnCheckButtonPressed;
    }

    public override void _ExitTree()
    {
        CheckButton.Pressed -= OnCheckButtonPressed;
    }

    public void Init(CheatCodeResource cheatCodeResource, CheatCodeToggleHandler onToggle, bool isActivated = false)
    {
        _cheatCodeResource = cheatCodeResource;
        _onToggle = onToggle;

        Name.Text = cheatCodeResource.Name;
        Icon.Texture = cheatCodeResource.Icon;
        CheckButton.SetPressed(isActivated);
    }

    private void OnCheckButtonPressed()
    {
        _onToggle(_cheatCodeResource.Name, CheckButton.IsPressed());
    }
}