using System.Threading.Channels;

namespace SnekGameDevKit.Messaging;

public class MessageQueue(IMessageDisplay messageDisplay)
{
    readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

    /// <summary>任意线程调用都安全；入队后尽快触发显示。</summary>
    public void Enqueue(string message) => _channel.Writer.TryWrite(message);

    /// <summary>在 UI/主线程启动；每条消息到达立即转发给显示层（fire-and-forget 由实现层处理）。</summary>
    public async Task RunAsync(CancellationToken ct = default)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(ct))
        {
            messageDisplay.Fire(message); // 同步转发，无 discard；显示层自行 fire-and-forget
        }
    }
}
