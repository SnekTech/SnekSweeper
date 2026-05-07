using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Widgets;
using Widgets.MessageQueue;

namespace SnekSweeper.Autoloads;

[SceneTree]
public partial class MessageBox : Control, IMessageDisplay
{
    const float MessageLifetime = 3;
    MessageQueue _messageQueue = null!;

    static MessageBox Instance { get; set; } = null!;

    public static void Print(string message) => Instance._messageQueue.Enqueue(message);

    public override void _Ready()
    {
        Instance = this;

        _messageQueue = new MessageQueue(this);
        _messageQueue.StartRunning(this.GetCancellationTokenOnTreeExit()).AsGDTask().Forget();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.IsReleased() && eventKey.Keycode == Key.M)
        {
            Print("Rider!");
        }
    }

    public void FireOneMessage(string message)
    {
        DisplayAsync().Forget();
        return;

        async GDTaskVoid DisplayAsync()
        {
            var messageLabel = new Label { Text = message };
            MessageContainer.AddChild(messageLabel);

            var cancelTokenOnTreeExit = this.GetCancellationTokenOnTreeExit();
            await GDTask.Delay(TimeSpan.FromSeconds(MessageLifetime), cancelTokenOnTreeExit);
            await messageLabel.FadeOutAsync(1, cancelTokenOnTreeExit);
            messageLabel.QueueFree();
        }
    }
}