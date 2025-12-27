namespace Widgets.MessageQueue;

public class MessageQueue(IMessageDisplay messageDisplay)
{
    readonly Queue<string> _queue = [];
    bool _isRunning;

    public float OutputIntervalSeconds { get; init; } = 1;

    public async Task StartRunning(CancellationToken token)
    {
        if (_isRunning)
        {
            Console.WriteLine("the queue is already running");
            return;
        }

        _isRunning = true;
        
        while (_isRunning)
        {
            token.ThrowIfCancellationRequested();
            FireOneMessage();
            await Task.Delay(TimeSpan.FromSeconds(OutputIntervalSeconds), token);
        }
    }

    public void StopRunning()
    {
        _isRunning = false;
    }

    public void Enqueue(string message)
    {
        _queue.Enqueue(message);
    }

    void FireOneMessage()
    {
        if (_queue.Count == 0)
            return;

        var message = _queue.Dequeue();
        messageDisplay.FireOneMessage(message);
    }
}