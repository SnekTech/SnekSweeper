using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

public class GameRunRecorder(
    Func<CurrentRunInfo> getCurrentRunInfo,
    Action<Func<CurrentRunInfo, CurrentRunInfo>> updateCurrentRunInfo,
    Action<Func<History, History>> updateHistory)
{
    RunStartInfo StartInfo => getCurrentRunInfo().StartInfo;

    public void MarkRunStartInfo(RunStartInfo startInfo) =>
        updateCurrentRunInfo(r => r with { StartInfo = startInfo });

    public GameRunRecord GenerateRecentRecord(bool winning, bool[,] bombs) => GameRunRecord.Create(
        RunDuration.Create(StartInfo.StartAt, DateTime.Now),
        winning,
        bombs,
        StartInfo.StartIndex
    );

    public void SaveRecord(GameRunRecord runRecord) => updateHistory(h => h.Add(runRecord));

    public void UpdateGridSnapshot(Grid grid) =>
        updateCurrentRunInfo(r => r with { GridSnapshot = grid.GetSnapshot() });

    public void ClearSnapshot() =>
        updateCurrentRunInfo(r => r with { GridSnapshot = null });
}

public readonly record struct RunStartInfo(DateTime StartAt, GridIndex StartIndex);
