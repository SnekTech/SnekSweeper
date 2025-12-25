namespace SnekSweeper.Autoloads;

public partial class QuitHandler : Node
{
    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest) return;

        GD.Print("Saving player data json...");
        HouseKeeper.SaveCurrentPlayerData();

        GD.Print("Bye");
        GetTree().Quit();
    }
}