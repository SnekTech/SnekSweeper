using Godot;
using GodotUtilities;
using SnekSweeper.Autoloads;

namespace SnekSweeper.UI.History;

[Scene]
public partial class HistoryPage : CanvasLayer
{
    [Export] private PackedScene recordCardScene = null!;
    
    [Node] private Label recordsCountLabel = null!;
    [Node] private VBoxContainer recordsContainer = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var records = HouseKeeper.History.Records;
        recordsCountLabel.Text = $"{records.Count} records in total";
        
        foreach (var record in records)
        {
            var recordCard = recordCardScene.Instantiate<RecordCard>();
            recordCard.SetContent(record);
            recordsContainer.AddChild(recordCard);
        }
    }
}