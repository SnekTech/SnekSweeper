using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeperCore.LevelManagement;

/// <summary>
/// 一局游戏的 save-data 生命周期协调器（无内部状态）：FSM 按阶段调用
/// （首次输入处理完成创建 OngoingGame → 持续更新快照 → 结束清空）。
/// startInfo 由首次输入处理完成时刻构造（新局），续局保留存档最初值。
/// </summary>
public class GameRunRecorder(ISaveDataStore store)
{
    /// <summary>新局：首次输入处理完成，创建 OngoingGame（startInfo + snapshot 同时就绪）。</summary>
    public void StartOngoingGame(GridSnapshot snapshot, RunStartInfo startInfo)
        => store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
            { OngoingGame = new OngoingGame(snapshot, startInfo) }));

    /// <summary>续局/进行中：只更新快照，保留最初 startInfo。</summary>
    public void UpdateOngoingGame(GridSnapshot snapshot)
        => store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
            { OngoingGame = r.OngoingGame! with { GridSnapshot = snapshot } }));

    /// <summary>结束：用 OngoingGame 的 startInfo 生成记录并清空（首击即负也已创建，无 fallback）。</summary>
    public GameRunRecord FinishRun(bool winning, bool[,] bombs)
    {
        var record = GameRunRecord.FromRun(store.State.CurrentRunInfo.OngoingGame!.StartInfo,
            DateTime.Now, winning, bombs);
        store.Dispatch(s => s.UpdateHistory(h => h.Add(record)));
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with { OngoingGame = null }));
        return record;
    }
}

public readonly record struct RunStartInfo(DateTime StartAt, GridIndex StartIndex);
