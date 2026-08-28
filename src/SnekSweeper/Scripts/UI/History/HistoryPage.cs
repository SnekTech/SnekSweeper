using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using GodotGadgets.Extensions;
using GodotGadgets.UI.Pagination;
using SnekSweeper.Widgets;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.UI.History;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HistoryPage : CanvasLayer, ISceneScript
{
    const int RunRecordPageSize = 2;

    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    public void OnResolved()
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
        var records = SaveData.State.History.Records
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
