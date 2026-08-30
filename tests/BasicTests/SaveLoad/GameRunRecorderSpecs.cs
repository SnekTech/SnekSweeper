using AwesomeAssertions;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// OngoingGame 重构后的 recorder 行为（无内部状态）：
/// StartOngoingGame 首次输入处理完成创建；UpdateOngoingGame 只更新快照保留最初 startInfo；
/// FinishRun 用 OngoingGame 的开局信息生成记录并清空。
/// </summary>
public sealed class GameRunRecorderSpecs
{
    [Test]
    public void start_ongoing_game_creates_ongoing_game_with_start_info()
    {
        var store = new FakeSaveDataStore(PlayerSaveData.CreateEmpty());
        var recorder = new GameRunRecorder(store);

        recorder.StartOngoingGame(SampleGridSnapshot(), SampleStartInfo());

        var ongoing = store.State.CurrentRunInfo.OngoingGame;
        ongoing.Should().NotBeNull();
        // GridSnapshot 含 2D 数组，record Equals 是引用相等 → 用 BeEquivalentTo 做结构比较
        ongoing.GridSnapshot.Should().BeEquivalentTo(SampleGridSnapshot());
        ongoing.StartInfo.Should().Be(SampleStartInfo());
    }

    [Test]
    public void update_ongoing_game_updates_snapshot_and_keeps_start_info()
    {
        var store = new FakeSaveDataStore(PlayerSaveData.CreateEmpty());
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
        {
            OngoingGame = new OngoingGame(SampleGridSnapshot(), SampleStartInfo()),
        }));
        var recorder = new GameRunRecorder(store);

        recorder.UpdateOngoingGame(RevealedGridSnapshot());

        var ongoing = store.State.CurrentRunInfo.OngoingGame;
        ongoing.Should().NotBeNull();
        ongoing.GridSnapshot.Should().BeEquivalentTo(RevealedGridSnapshot());
        // 续局/进行中：只更新快照，保留最初 startInfo
        ongoing.StartInfo.Should().Be(SampleStartInfo());
    }

    [Test]
    public void finish_run_uses_ongoing_game_start_info_and_clears_it()
    {
        var store = new FakeSaveDataStore(PlayerSaveData.CreateEmpty());
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
        {
            OngoingGame = new OngoingGame(SampleGridSnapshot(), SampleStartInfo()),
        }));
        var recorder = new GameRunRecorder(store);

        var record = recorder.FinishRun(winning: true, bombs: BombMatrix());

        record.Duration.StartAt.Should().Be(SampleStartInfo().StartAt);
        record.StartIndex.Should().Be(SampleStartInfo().StartIndex);
        record.Winning.Should().BeTrue();
        store.State.History.Records.Should().ContainSingle();
        // 收尾清空 OngoingGame，杜绝残留的"进行中的局"
        store.State.CurrentRunInfo.OngoingGame.Should().BeNull();
    }

    static RunStartInfo SampleStartInfo() => new(DateTime.UnixEpoch, new GridIndex(1, 2));

    static GridSnapshot SampleGridSnapshot() => new(
        new[,]
        {
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
        },
        BombMatrix());

    static GridSnapshot RevealedGridSnapshot() => new(
        new[,]
        {
            { CellSnapshotState.Revealed, CellSnapshotState.Revealed },
            { CellSnapshotState.Revealed, CellSnapshotState.Revealed },
        },
        BombMatrix());

    static bool[,] BombMatrix() => new[,] { { false, false }, { false, false } };
}

sealed class FakeSaveDataStore(PlayerSaveData initialState) : ISaveDataStore
{
    public PlayerSaveData State { get; private set; } = initialState;

    public void Dispatch(Func<PlayerSaveData, PlayerSaveData> reduce) => State = reduce(State);

    public event Action? SavedFeedback;

    public void NotifySaved() => SavedFeedback?.Invoke();
}
