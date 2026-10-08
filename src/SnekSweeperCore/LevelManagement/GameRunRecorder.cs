using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeperCore.LevelManagement;

public class GameRunRecorder(ISaveDataStore store)
{
    public void SaveOngoingRun(OngoingGame ongoingGame)
        => store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
        {
            OngoingGame = ongoingGame,
        }));

    public void FinishRun(GameRunRecord latestRun)
    {
        store.Dispatch(s => s.UpdateHistory(h => h.Add(latestRun)));
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with { OngoingGame = null }));
    }
}

public readonly record struct RunStartInfo(DateTime StartAt, GridIndex StartIndex);