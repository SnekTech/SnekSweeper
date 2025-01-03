using Godot;
using GodotUtilities;
using Widgets;
using Widgets.MessageQueue;

namespace SnekSweeper.UI.MessageBox;

[Scene]
public partial class MessageBoxControl : Control
{
    [Node] private Button sendButton = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    private readonly MessageQueue _messageQueue = new(new ConsoleMessageDisplay());

    public override void _Ready()
    {
        _messageQueue.StartRunning().Fire();

        sendButton.Pressed += () => _messageQueue.Enqueue("Rider!");
    }
}