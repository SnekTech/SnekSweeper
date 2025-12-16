using SnekSweeper.GameHistory;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class RecordCard : PanelContainer, ISceneScript
{
    public void SetContent(GameRunRecord gameRunRecord)
    {
        SetTimeLabel(gameRunRecord.Duration);
        SetWinningLabel(gameRunRecord.Winning);
    }

    void SetTimeLabel(RunDuration duration)
    {
        TimeLabel.Text = $"from {duration.StartAt} to {duration.EndAt}";
    }

    void SetWinningLabel(bool winning)
    {
        var (labelText, labelColor) = winning switch
        {
            true => ("Success", Colors.Green),
            false => ("Fail", Colors.Red),
        };
        (WinningLabel.Text, WinningLabel.Modulate) = (labelText, labelColor);
    }
}