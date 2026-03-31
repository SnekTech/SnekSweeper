using System.Threading.Channels;

namespace Widgets.MessageQueue;

public class MessageQueue(IMessageDisplay messageDisplay)
{
    readonly Channel<string> _channel =
        Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
    bool _isRunning;

    public float OutputIntervalSeconds { get; init; } = 1;

    public async Task StartRunning(CancellationToken ct)
    {
        if (_isRunning)
        {
            Console.WriteLine("the queue is already running");
            return;
        }

        _isRunning = true;

        while (_isRunning)
        {
            ct.ThrowIfCancellationRequested();

            messageDisplay.FireOneMessage(await _channel.Reader.ReadAsync(ct));
            await Task.Delay(TimeSpan.FromSeconds(OutputIntervalSeconds), ct);
        }
    }

    public void Enqueue(string message) => _channel.Writer.TryWrite(message);
}