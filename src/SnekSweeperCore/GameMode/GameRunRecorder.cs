using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.GameMode;

public class GameRunRecorder(History history)
{
    DateTime CurrentRunStartAt { get; set; } = DateTime.MinValue;
    GridIndex StartIndex { get; set; }

    internal void MarkRunStartInfo(DateTime startAt, GridIndex startIndex) =>
        (CurrentRunStartAt, StartIndex) = (startAt, startIndex);

    public void CreateAndSaveRecord(bool winning, bool[,] bombs)
    {
        var record = GameRunRecord.Create(
            RunDuration.Create(CurrentRunStartAt, DateTime.Now),
            winning,
            bombs,
            StartIndex
        );
        history.AddRecord(record);
    }
}