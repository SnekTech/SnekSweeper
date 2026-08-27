using System.Threading.Channels;

namespace SnekGameDevKit;

/// <summary>
/// 单写者串行落盘队列：任意线程入队最新快照，后台单消费者按序写盘。
/// 消费时合并突发写入（只保留最新快照），保证写顺序 = 入队顺序，且磁盘最终态必为最新快照。
/// </summary>
public sealed class SaveQueue<T>(Func<T, CancellationToken, Task> writeAsync)
{
    readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    /// <summary>任意线程调用都安全；入队最新快照，尽快触发一次写盘。</summary>
    public void RequestSave(T state) => _channel.Writer.TryWrite(state);

    /// <summary>后台启动；串行消费，同一批突发请求只写最新一个。</summary>
    public async Task RunAsync(CancellationToken ct = default)
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(ct))
        {
            var latest = item;
            // 合并突发：写盘前把已积压的请求全部取出，只保留最新快照
            while (_channel.Reader.TryRead(out var newer))
                latest = newer;

            await writeAsync(latest, ct);
        }
    }
}
