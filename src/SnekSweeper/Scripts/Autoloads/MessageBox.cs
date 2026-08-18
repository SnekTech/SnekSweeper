using GodotGadgets.Tasks;
using GodotGadgets.TweenStuff;
using GodotTask;
using SnekGameDevKit.Messaging;

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
        _messageQueue.RunAsync(this.GetCancellationTokenOnTreeExit()).AsGDTask().Forget();

        SaveData.Instance.SavedFeedback += () => Print("已保存");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.IsReleased() && eventKey.Keycode == Key.M)
        {
            Print("Rider!");
        }
    }

    public void Fire(string message)
    {
        // fire-and-forget 在 Godot 层用 GDTask 的正确 Forget：异常当场进错误面板，OCE 默认吞掉
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
