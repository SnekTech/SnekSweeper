using Godot;
using GodotUtilities;
using SnekSweeper.CheatCode;

namespace SnekSweeper.UI.CheatCode;

[Scene]
public partial class CheatCodeCard : PanelContainer
{
    [Node] private Label name = null!;
    [Node] private TextureRect icon = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }
    
    public void SetContent(CheatCodeResource cheatCodeResource)
    {
        name.Text = cheatCodeResource.Name;
        icon.Texture = cheatCodeResource.Icon;
    }
}