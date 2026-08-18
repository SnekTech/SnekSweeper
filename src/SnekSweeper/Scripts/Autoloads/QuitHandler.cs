namespace SnekSweeper.Autoloads;

public partial class QuitHandler : Node
{
    static readonly CancellationTokenSource CancelOnQuit = new();
    public static CancellationToken QuitGameToken => CancelOnQuit.Token;
    
    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest) return;

        GD.Print("Saving player data...");
        // cannot use async save here,
        // because I cannot find the proper cancellation token,
        // the whole game is quiting
        SaveData.SaveNow();

        CancelOnQuit.Cancel();
        GD.Print("Bye");
        GetTree().Quit();
    }
}
