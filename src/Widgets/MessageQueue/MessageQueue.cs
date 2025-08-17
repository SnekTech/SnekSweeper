namespace Widgets.MessageQueue;

public class MessageQueue(IMessageDisplay messageDisplay)
{
    private readonly Queue<string> _queue = [];
    private bool _isRunning;

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
            OutputOneMessage(token);
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

    private void OutputOneMessage(CancellationToken token)
    {
        if (_queue.Count == 0)
            return;

        var message = _queue.Dequeue();
        messageDisplay.Display(message, token).Fire();
    }
}