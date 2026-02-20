using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.GameMode;

public class GameRunRecorder(History history)
{
    DateTime CurrentRunStartAt { get; set; } = DateTime.MinValue;
    GridIndex StartIndex { get; set; }

    internal void MarkRunStartInfo(DateTime startAt, GridIndex startIndex) =>
        (CurrentRunStartAt, StartIndex) = (startAt, startIndex);


    public GameRunRecord GenerateRecentRecord(bool winning, bool[,] bombs) => GameRunRecord.Create(
        RunDuration.Create(CurrentRunStartAt, DateTime.Now),
        winning,
        bombs,
        StartIndex
    );

    public void SaveRecord(GameRunRecord runRecord) => history.AddRecord(runRecord);
}