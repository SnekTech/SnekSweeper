namespace SnekSweeperCore.GameHistory;

public class History(List<GameRunRecord> records)
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public IReadOnlyList<GameRunRecord> Records { get; } = records;

    public void AddRecord(GameRunRecord gameRunRecord)
    {
        records.Add(gameRunRecord);
    }

    public void ClearRecords()
    {
        records.Clear();
    }
}