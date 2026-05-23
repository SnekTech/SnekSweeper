using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

public class GameRunRecorder(CurrentRunInfo currentRunInfo, History history)
{
    RunStartInfo StartInfo
    {
        get => currentRunInfo.StartInfo; 
        set => currentRunInfo.StartInfo = value;
    }

    public void MarkRunStartInfo(RunStartInfo startInfo) => StartInfo = startInfo;

    public GameRunRecord GenerateRecentRecord(bool winning, bool[,] bombs) => GameRunRecord.Create(
        RunDuration.Create(StartInfo.StartAt, DateTime.Now),
        winning,
        bombs,
        StartInfo.StartIndex
    );

    public void SaveRecord(GameRunRecord runRecord) => history.AddRecord(runRecord);
    
    public void UpdateGridSnapshot(Grid grid)
    {
        currentRunInfo.GridSnapshot = grid.GetSnapshot();
    }

    public void ClearSnapshot() => currentRunInfo.GridSnapshot = null;
}

public readonly record struct RunStartInfo(DateTime StartAt, GridIndex StartIndex);