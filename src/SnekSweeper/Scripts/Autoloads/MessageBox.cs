using GodotGadgets.Tasks;
using SnekSweeper.Widgets;
using Widgets.MessageQueue;

namespace SnekSweeper.Autoloads;

[SceneTree]
public partial class MessageBox : Control, IMessageDisplay
{
    const float MessageLifetime = 3;
    MessageQueue _messageQueue = null!;

    static MessageBox Instance { get; set; } = null!;

    public static void Print(string message)
    {
        Instance.Enqueue(message);
    }

    public override void _Ready()
    {
        Instance = this;

        _messageQueue = new MessageQueue(this)
        {
            OutputIntervalSeconds = 0.5f,
        };
        _messageQueue.StartRunning(this.GetCancellationTokenOnTreeExit()).Fire();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.IsReleased() && eventKey.Keycode == Key.M)
        {
            Enqueue("Rider!");
        }
    }

    public void FireOneMessage(string message)
    {
        DisplayAsync().Fire();
        return;

        async Task DisplayAsync()
        {
            var messageLabel = new Label { Text = message };
            MessageContainer.AddChild(messageLabel);

            var cancelTokenOnTreeExit = this.GetCancellationTokenOnTreeExit();
            await SnekUtility.DelayGd(MessageLifetime, cancelTokenOnTreeExit);
            await messageLabel.FadeOutAsync(1, cancelTokenOnTreeExit);
            messageLabel.QueueFree();
        }
    }

    void Enqueue(string message) => _messageQueue.Enqueue(message);
}