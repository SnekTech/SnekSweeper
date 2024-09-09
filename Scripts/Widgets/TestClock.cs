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
	}

	public override void _Process(double delta)
	{
		_time.Text = _clock.TimeElapsed.ToString(CultureInfo.InvariantCulture);
	}
}