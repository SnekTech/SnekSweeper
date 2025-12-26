using GodotGadgets.Tasks;
using SnekSweeper.Widgets;
using Widgets.MessageQueue;

namespace SnekSweeper.Autoloads;

[SceneTree]
public partial class MessageBox : Control, IMessageDisplay
{
    private const float MessageLifetime = 3;
    private MessageQueue _messageQueue = null!;

    private static MessageBox Instance { get; set; } = null!;

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
            _messageQueue.Enqueue("Rider!");
        }
    }

    public async Task Display(string message, CancellationToken token)
    {
        var messageLabel = new Label { Text = message };
        MessageContainer.AddChild(messageLabel);

        await SnekUtility.DelayGd(MessageLifetime, token);
        await messageLabel.FadeOutAsync(1, token);
    }

    private void Enqueue(string message) => _messageQueue.Enqueue(message);
}