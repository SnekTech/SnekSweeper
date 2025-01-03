namespace Widgets.MessageQueue;

public class MessageQueue(IMessageDisplay messageDisplay)
{
    private readonly Queue<string> _queue = [];
    private bool _isRunning;

    public int OutputIntervalSeconds { get; set; } = 1;

    public async Task StartRunning()
    {
        if (_isRunning)
        {
            Console.WriteLine("the queue is already running");
            return;
        }

        _isRunning = true;
        while (_isRunning)
        {
            OutputOneMessage();
            await Task.Delay(TimeSpan.FromSeconds(OutputIntervalSeconds));
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

    private void OutputOneMessage()
    {
        if (_queue.Count == 0)
            return;

        var message = _queue.Dequeue();
        messageDisplay.Display(message);
    }
}