using System.Globalization;
using Godot;

namespace SnekSweeper.Widgets;

public partial class TestClock : Node2D
{
    [Export]
    private Clock _clock = null!;

    [Export]
    private Label _time = null!;

    public override void _Ready()
    {
        _clock.Start();
        _time.Text = 0.ToString(CultureInfo.InvariantCulture);

        _clock.Ticked += OnClockTicked;
    }

    public override void _ExitTree()
    {
        _clock.Ticked -= OnClockTicked;
    }

    private void OnClockTicked()
    {
        _time.Text = _clock.TimeElapsed.ToString(CultureInfo.InvariantCulture);
    }
}