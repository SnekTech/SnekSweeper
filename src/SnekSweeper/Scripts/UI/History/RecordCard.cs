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
        SetSeedLabel(gameRunRecord.RngData.Seed);
    }

    void SetTimeLabel(RunDuration duration)
    {
        TimeLabel.Text = $"from {duration.StartAt} to {duration.EndAt}";
    }

    void SetWinningLabel(bool winning)
    {
        if (winning)
        {
            WinningLabel.Text = "Success";
            WinningLabel.Modulate = Colors.Green;
        }
        else
        {
            WinningLabel.Text = "Fail";
            WinningLabel.Modulate = Colors.Red;
        }
    }

    void SetSeedLabel(ulong seed)
    {
        SeedLabel.Text = $"seed: {seed}";
    }
}