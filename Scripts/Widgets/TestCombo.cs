using Godot;

namespace SnekSweeper.Widgets;

public partial class TestCombo : Node2D
{
    [Export] private ProgressBar progressBar = default!;
    [Export] private Button button = default!;
    [Export] private Label descriptionLabel = default!;
    [Export] private ComboComponent comboComponent = default!;

    private Color[] _fillColors = { Colors.Gray, Colors.White, Colors.Yellow, Colors.Crimson };

    public override void _Ready()
    {
        button.Pressed += OnButtonPressed;
        comboComponent.LevelRefreshed += OnComboLevelRefreshed;
        comboComponent.LevelDropped += OnComboLevelDropped;
    }

    public override void _ExitTree()
    {
        button.Pressed -= OnButtonPressed;
        comboComponent.LevelRefreshed -= OnComboLevelRefreshed;
        comboComponent.LevelDropped -= OnComboLevelDropped;
    }

    public override void _Process(double delta)
    {
        if (!comboComponent.IsCountingCombo)
        {
            return;
        }

        progressBar.Value = progressBar.MaxValue * comboComponent.ComboAlivePercent;
    }

    private void UpdateProgressColor()
    {
        var level = comboComponent.CurrentLevel;
        progressBar.Modulate = _fillColors[level];

        descriptionLabel.Text = comboComponent.CurrentComment;
        descriptionLabel.Modulate = _fillColors[level];
    }

    private void OnButtonPressed()
    {
        comboComponent.Strike();
    }

    private void OnComboLevelRefreshed()
    {
        UpdateProgressColor();
        progressBar.Value = progressBar.MaxValue;
    }

    private void OnComboLevelDropped()
    {
        UpdateProgressColor();
        if (!comboComponent.IsCountingCombo)
        {
            progressBar.Value = 0;
            descriptionLabel.Text = "";
        }
    }
}