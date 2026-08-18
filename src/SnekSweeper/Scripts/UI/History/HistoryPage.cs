using GodotGadgets.Extensions;
using GodotGadgets.UI.Pagination;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;

namespace SnekSweeper.UI.History;

[SceneTree]
public partial class HistoryPage : CanvasLayer, ISceneScript
{
    const int RunRecordPageSize = 2;
    
    public override void _Ready()
    {
        ResetRunRecords();
    }

    public override void _EnterTree()
    {
        ClearButton.Pressed += OnClearButtonPressed;
    }

    public override void _ExitTree()
    {
        ClearButton.Pressed -= OnClearButtonPressed;
    }

    void ResetRunRecords()
    {
        RecordsContainer.ClearChildren();
        InitPagination();
    }

    void InitPagination()
    {
        var records = SaveData.History.Records
            .OrderByDescending(r => r.Duration.EndAt).ToList();
        var historyQuery = new HistoryQuery(records);
        var pagination = new Pagination<GameRunRecord>(historyQuery, RunRecordPageSize);
        RunRecordPaginationBar.Bind(
            RecordsContainer,
            runRecord =>
            {
                var card = RecordCard.Instantiate();
                card.RunRecord = runRecord;
                return card;
            },
            pagination);
    }

    void OnClearButtonPressed()
    {
        SaveData.UpdateHistory(h => h.Clear());
        SaveData.NotifySaved();
        ResetRunRecords();
    }
}
