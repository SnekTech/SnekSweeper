using System;
using Godot;
using GodotUtilities;
using SnekSweeper.GameHistory;

namespace SnekSweeper.UI.History;

[Scene]
public partial class RecordCard : PanelContainer
{
    [Node] private Label timeLabel = null!;
    [Node] private Label winningLabel = null!;
    [Node] private Label seedLabel = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public void SetContent(Record record)
    {
        SetTimeLabel(record.StartAt, record.EndAt);
        SetWinningLabel(record.Winning);
        SetSeedLabel(record.RandomGeneratorData.Seed);
    }

    private void SetTimeLabel(DateTime startAt, DateTime endAt)
    {
        timeLabel.Text = $"from {startAt} to {endAt}";
    }

    private void SetWinningLabel(bool winning)
    {
        if (winning)
        {
            winningLabel.Text = "Success";
            winningLabel.Modulate = Colors.Green;
        }
        else
        {
            winningLabel.Text = "Fail";
            winningLabel.Modulate = Colors.Red;
        }
    }

    private void SetSeedLabel(ulong seed)
    {
        seedLabel.Text = $"seed: {seed}";
    }
}