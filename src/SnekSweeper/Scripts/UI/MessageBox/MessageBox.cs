using Godot;
using GodotUtilities;
using SnekSweeper.Widgets;
using Widgets;
using Widgets.MessageQueue;

namespace SnekSweeper.UI.MessageBox;

[Scene]
public partial class MessageBox : Control, IMessageDisplay
{
    [Node] private Button sendButton = null!;
    [Node] private VBoxContainer messageContainer = null!;

    private const float MessageLifetime = 3;
    private MessageQueue _messageQueue = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        _messageQueue = new MessageQueue(this)
        {
            OutputIntervalSeconds = 0.5f
        };
        _messageQueue.StartRunning().Fire();

        sendButton.Pressed += () => _messageQueue.Enqueue("Rider!");
    }

    public void Display(string message)
    {
        var messageLabel = new Label { Text = message };
        messageContainer.AddChild(messageLabel);

        messageLabel.FadeOutAsync(MessageLifetime, 1, messageLabel.QueueFree).Fire();
    }
}