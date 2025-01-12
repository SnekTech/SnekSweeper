using Godot;

namespace SnekSweeper.Autoloads;

public partial class QuitHandler : Node
{
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GD.Print("Bye");
            
            GetTree().Quit();
        }
    }
}