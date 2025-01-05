namespace Widgets.MessageQueue;

public interface IMessageDisplay
{
    Task Display(string message);
}