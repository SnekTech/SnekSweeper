using GodotGadgets.Extensions;
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

    public override void _EnterTree()
    {
        ClearButton.Pressed += OnClearButtonPressed;
    }

    public override void _ExitTree()
    {
        ClearButton.Pressed -= OnClearButtonPressed;
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

    private void OnClearButtonPressed()
    {
        HouseKeeper.History.ClearRecords();
        RecordsContainer.ClearChildren();
    }
}