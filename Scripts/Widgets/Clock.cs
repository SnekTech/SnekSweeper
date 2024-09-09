using Godot;

namespace SnekSweeper.Widgets;

[GlobalClass]
public partial class Clock : Node
{
    private bool _running;

    public override void _Process(double delta)
    {
        if (!_running) return;

        TimeElapsed += delta;
    }

    public void Start()
    {
        TimeElapsed = 0;
        _running = true;
    }

    public void Stop()
    {
        _running = false;
    }

    public double TimeElapsed { get; private set; }
}