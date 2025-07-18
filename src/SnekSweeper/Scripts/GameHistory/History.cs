using SnekSweeper.SaveLoad;

namespace SnekSweeper.GameHistory;

public class History
{
    public List<GameRunRecord> Records { get; } = [];

    public void AddRecord(GameRunRecord gameRunRecord)
    {
        Records.Add(gameRunRecord);
        SaveLoadEventBus.EmitSaveRequested();
    }

    public void ClearRecords()
    {
        Records.Clear();
        SaveLoadEventBus.EmitSaveRequested();
    }
}