using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class RecordCard : PanelContainer, ISceneScript
{
    public void SetContent(GameRunRecord gameRunRecord)
    {
        SetTimeLabel(gameRunRecord.Duration);
        SetWinningLabel(gameRunRecord.Winning);
        SetStartIndexLabel(gameRunRecord.StartIndex);
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

    void SetStartIndexLabel(GridIndex gridIndex)
    {
        StartIndexLabel.Text = gridIndex.ToString();
    }
}