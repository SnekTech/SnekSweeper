using Godot;
using GodotUtilities;
using SnekSweeper.CheatCode;

namespace SnekSweeper.UI.CheatCode;

[Scene]
public partial class CheatCodeCard : PanelContainer
{
    [Node] private Label name = null!;
    [Node] private TextureRect icon = null!;
    [Node] private CheckButton checkButton = null!;
    
    public delegate void CheatCodeToggleHandler(string cheatCodeName, bool isButtonChecked);

    private CheatCodeToggleHandler _onToggle = null!;
    private CheatCodeResource _cheatCodeResource = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _ExitTree()
    {
        checkButton.Pressed -= OnCheckButtonPressed;
    }

    public void Init(CheatCodeResource cheatCodeResource, CheatCodeToggleHandler onToggle, bool isActivated = false)
    {
        _cheatCodeResource = cheatCodeResource;
        _onToggle = onToggle;

        name.Text = cheatCodeResource.Name;
        icon.Texture = cheatCodeResource.Icon;
        checkButton.SetPressed(isActivated);

        checkButton.Pressed += OnCheckButtonPressed;
    }

    private void OnCheckButtonPressed()
    {
        _onToggle(_cheatCodeResource.Name, checkButton.IsPressed());
    }
}