using Godot;

namespace SnekSweeper.Widgets;

public partial class TestCombo : Node2D
{
    [Export]
    private ProgressBar _progressBar = null!;

    [Export]
    private Button _button = null!;

    [Export]
    private Timer _comboTimer = null!;

    private const int ComboWaitTime = 3;
    private int _comboLevel;
    private bool IsCountingCombo => !_comboTimer.IsStopped();
    
    // private StyleBoxFlat _fillStyleBox = new();

    public override void _Ready()
    {
        // _fillStyleBox.BgColor = Colors.Aqua;
        // _progressBar.AddThemeStyleboxOverride("fill", _fillStyleBox);
        
        _button.Pressed += HandleClick;
        _comboTimer.Timeout += HandleComboTimerTimeout;
    }

    private void HandleComboTimerTimeout()
    {
        _comboLevel = 0;
    }

    public override void _Process(double delta)
    {
        if (!IsCountingCombo)
            return;
        
        _progressBar.Value = _comboTimer.TimeLeft / ComboWaitTime * _progressBar.MaxValue;
    }

    private void HandleClick()
    {
        _progressBar.Value = _progressBar.MaxValue;
        _comboTimer.Start(ComboWaitTime);
        _comboLevel++;
        GD.Print(_comboLevel);
    }
}