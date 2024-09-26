using System.Globalization;
using Godot;
using SnekSweeper.Autoloads;

namespace SnekSweeper.UI;

public partial class HistoryScene : Node2D
{
    public override void _Ready()
    {
        PrintHistoryRecords();
    }

    private void PrintHistoryRecords()
    {
        foreach (var record in HouseKeeper.History.Records)
        {
            var culture = CultureInfo.CurrentCulture;
            var (startAt, endAt, winning) =
                (record.StartAt.ToString(culture), record.EndAt.ToString(culture), record.Winning);
            GD.Print($"from {startAt} to {endAt}, {winning}");
        }
    }
}