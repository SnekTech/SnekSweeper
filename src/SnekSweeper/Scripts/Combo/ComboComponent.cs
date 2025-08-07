namespace SnekSweeper.Combo;

[SceneTree]
public partial class ComboComponent : Node
{
    private const int MaxComboLevel = 4;
    private const float DefaultComboInterval = 5.0f;

    private int _level;
    private IComboDisplay? _comboDisplay;

    public IComboDisplay? ComboDisplay
    {
        get => _comboDisplay;
        set
        {
            _comboDisplay = value;
            UpdateLevelText();
            UpdateTimerProgress();
        }
    }

    private int Level
    {
        get => _level;
        set
        {
            _level = Mathf.Clamp(value, 0, MaxComboLevel);
            UpdateLevelText();
        }
    }

    #region lifecycle

    public override void _EnterTree()
    {
        ComboTimer.Timeout += OnComboTimerTimeout;
    }

    public override void _Ready()
    {
        Reset();
    }

    public override void _ExitTree()
    {
        ComboTimer.Timeout -= OnComboTimerTimeout;
    }


    public override void _Process(double delta)
    {
        if (ComboTimer.IsStopped()) return;

        UpdateTimerProgress();
    }

    #endregion

    private void Reset()
    {
        Level = 0;
        ComboTimer.Stop();
        UpdateTimerProgress();
    }

    public void IncreaseComboLevel()
    {
        Level++;
        ComboTimer.Start(DefaultComboInterval);
        UpdateTimerProgress();
    }

    private void DecreaseComboLevel()
    {
        if (Level <= 0) return;

        Level--;

        if (Level > 1) return;

        Reset();
    }

    private void UpdateTimerProgress() => ComboDisplay?.DisplayProgress(ComboTimer.TimeLeft / ComboTimer.WaitTime);
    private void UpdateLevelText() => ComboDisplay?.DisplayLevelText(GetComboLevelText(_level));

    private void OnComboTimerTimeout() => DecreaseComboLevel();

    private static string GetComboLevelText(int level) =>
        level switch
        {
            <= 1 => string.Empty,
            2 => "Good",
            3 => "Great",
            MaxComboLevel => "Excellent",
            _ => "Overflow!",
        };
}