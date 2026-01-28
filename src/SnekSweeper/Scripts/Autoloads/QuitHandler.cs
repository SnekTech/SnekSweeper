namespace SnekSweeper.Autoloads;

public partial class QuitHandler : Node
{
    readonly CancellationTokenSource cancelOnQuit = new();
    public CancellationToken QuitGameToken => cancelOnQuit.Token;
    
    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest) return;

        GD.Print("Saving player data...");
        HouseKeeper.SaveCurrentPlayerData();

        cancelOnQuit.Cancel();
        GD.Print("Bye");
        GetTree().Quit();
    }
}