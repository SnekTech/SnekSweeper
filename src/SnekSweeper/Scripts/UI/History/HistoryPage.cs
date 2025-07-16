using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class HistoryPage : CanvasLayer, ISceneScript
{
    public override void _Ready()
    {
        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var records = HouseKeeper.History.Records;
        RecordsCountLabel.Text = $"{records.Count} records in total";
        
        foreach (var record in records)
        {
            var recordCard = SceneFactory.Instantiate<RecordCard>();
            recordCard.SetContent(record);
            RecordsContainer.AddChild(recordCard);
        }
    }
}