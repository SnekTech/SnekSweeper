using Godot;
using Widgets.MessageQueue;

namespace SnekSweeper.UI.MessageBox;

public class ConsoleMessageDisplay : IMessageDisplay
{
    public void Display(string message)
    {
        GD.Print(message);
    }
}