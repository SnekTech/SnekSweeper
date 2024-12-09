using System.Globalization;
using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;
using SnekSweeper.Constants;

namespace SnekSweeper.UI;

[Scene]
public partial class HistoryPage : CanvasLayer
{
    [Node] private Label recordsCountLabel = null!;
    [Node] private VBoxContainer recordsContainer = null!;
    [Node] private Button backToMainButton = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        backToMainButton.Pressed += () => SceneManager.Instance.GotoScene(SceneName.Main);

        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var records = HouseKeeper.History.Records;
        recordsCountLabel.Text = $"{records.Count} records in total";
        
        foreach (var record in records)
        {
            var culture = CultureInfo.CurrentCulture;
            var (startAt, endAt, winning) =
                (record.StartAt.ToString(culture), record.EndAt.ToString(culture), record.Winning);
            var recordString = $"from {startAt} to {endAt}, {winning}";

            var label = new Label();
            label.Text = recordString;
            recordsContainer.AddChild(label);
        }
    }
}