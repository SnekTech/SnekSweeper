using System;
using Godot;

namespace SnekSweeper.Widgets;

[GlobalClass]
public partial class Clock : Node
{
    public event Action? Ticked;
    
    private bool _running;
    private double _interval = 1;
    private double _intervalElapsed;
    
    public override void _Process(double delta)
    {
        if (!_running) return;

        TimeElapsed += delta;
        
        _intervalElapsed += delta;
        if (_intervalElapsed >= _interval)
        {
            _intervalElapsed -= _interval;
            Ticked?.Invoke();
        }
    }

    public void Start(double interval = 1)
    {
        TimeElapsed = 0;
        _intervalElapsed = 0;
        _interval = interval;
        
        _running = true;
    }

    public void Stop()
    {
        _running = false;
    }

    public double TimeElapsed { get; private set; }
}