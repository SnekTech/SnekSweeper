using GodotGadgets.Tasks;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class RecordCard : PanelContainer, ISceneScript
{
    public GameRunRecord RunRecord
    {
        get;
        set
        {
            field = value;
            SetTimeLabel(value.Duration);
            SetWinningLabel(value.Winning);
            SetStartIndexLabel(value.StartIndex);
        }
    } = null!;

    public override void _EnterTree()
    {
        RetryButton.Pressed += OnRetryButtonPressed;
    }

    public override void _ExitTree()
    {
        RetryButton.Pressed -= OnRetryButtonPressed;
    }

    void SetTimeLabel(RunDuration duration) => TimeLabel.Text = $"from {duration.StartAt} to {duration.EndAt}";

    void SetWinningLabel(bool winning)
    {
        var (labelText, labelColor) = winning switch
        {
            true => ("Success", Colors.Green),
            false => ("Fail", Colors.Red),
        };
        (WinningLabel.Text, WinningLabel.Modulate) = (labelText, labelColor);
    }

    void SetStartIndexLabel(GridIndex gridIndex) => StartIndexLabel.Text = gridIndex.ToString();

    void OnRetryButtonPressed()
    {
        var loadLevelSource = new FromRunRecord(RunRecord);
        Autoload.SceneSwitcher.LoadLevel(loadLevelSource).Fire();
    }
}