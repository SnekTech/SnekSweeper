using SnekSweeper.GameHistory;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class RecordCard : PanelContainer, ISceneScript
{
    public void SetContent(GameRunRecord gameRunRecord)
    {
        SetTimeLabel(gameRunRecord.StartAt, gameRunRecord.EndAt);
        SetWinningLabel(gameRunRecord.Winning);
        SetSeedLabel(gameRunRecord.RandomData.Seed);
    }

    private void SetTimeLabel(DateTime startAt, DateTime endAt)
    {
        TimeLabel.Text = $"from {startAt} to {endAt}";
    }

    private void SetWinningLabel(bool winning)
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

    private void SetSeedLabel(ulong seed)
    {
        SeedLabel.Text = $"seed: {seed}";
    }
}