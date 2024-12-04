using System;
using Godot;

namespace SnekSweeper.Widgets;

public partial class ComboComponent : Node
{
    [Export] private Timer _timer = default!;

    public event Action? LevelRefreshed;
    public event Action? LevelDropped;

    private const float ComboWaitTime = 1;
    private const int MaxComboLevel = 3;

    private readonly string[] _comboComments = { "good", "great", "awesome", "unbelievable" };

    public int CurrentLevel { get; private set; }
    public bool IsCountingCombo { get; private set; }
    public float ComboAlivePercent => (float)_timer.TimeLeft / ComboWaitTime;
    public string CurrentComment => _comboComments[CurrentLevel];

    public override void _Ready()
    {
        _timer.Timeout += OnTimeout;
    }

    public override void _ExitTree()
    {
        _timer.Timeout -= OnTimeout;
    }

    public void Strike()
    {
        if (!IsCountingCombo)
        {
            IsCountingCombo = true;
        }
        else
        {
            CurrentLevel = Mathf.Min(CurrentLevel + 1, MaxComboLevel);
        }

        _timer.Start(ComboWaitTime);
        LevelRefreshed?.Invoke();
    }

    private void OnTimeout()
    {
        if (CurrentLevel > 0)
        {
            CurrentLevel = Mathf.Max(CurrentLevel - 1, 0);
            _timer.Start(ComboWaitTime);
        }
        else
        {
            IsCountingCombo = false;
        }

        LevelDropped?.Invoke();
    }
}