using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeperCore.LevelManagement;

public class GameRunRecorder(ISaveDataStore store)
{
    RunStartInfo StartInfo => store.State.CurrentRunInfo.StartInfo;

    public void MarkRunStartInfo(RunStartInfo startInfo) =>
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with { StartInfo = startInfo }));

    public void UpdateGridSnapshot(Grid grid) =>
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with { GridSnapshot = grid.GetSnapshot() }));

    public GameRunRecord FinishRun(bool winning, bool[,] bombs)
    {
        var record = GameRunRecord.FromRun(StartInfo, DateTime.Now, winning, bombs);
        store.Dispatch(s => s.UpdateHistory(h => h.Add(record)));
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with { GridSnapshot = null }));
        return record;
    }
}

public readonly record struct RunStartInfo(DateTime StartAt, GridIndex StartIndex);
