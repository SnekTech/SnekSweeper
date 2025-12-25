namespace SnekSweeper.GameHistory;

public class History
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<GameRunRecord> Records { get; set; } = [];

    public void AddRecord(GameRunRecord gameRunRecord)
    {
        Records.Add(gameRunRecord);
    }

    public void ClearRecords()
    {
        Records.Clear();
    }
}