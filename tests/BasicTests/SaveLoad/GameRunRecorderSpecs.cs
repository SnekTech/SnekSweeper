using AwesomeAssertions;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

public sealed class GameRunRecorderSpecs
{
    [Test]
    public void save_ongoing_run_writes_ongoing_game()
    {
        var store = new FakeSaveDataStore(PlayerSaveData.CreateEmpty());
        var recorder = new GameRunRecorder(store);

        recorder.SaveOngoingRun(new OngoingGame(SampleGridSnapshot(), SampleStartInfo));

        var ongoing = store.State.CurrentRunInfo.OngoingGame;
        ongoing.Should().NotBeNull();
        // GridSnapshot 含 2D 数组，record Equals 是引用相等 → 用 BeEquivalentTo 做结构比较
        ongoing.GridSnapshot.Should().BeEquivalentTo(SampleGridSnapshot());
        ongoing.StartInfo.Should().Be(SampleStartInfo);
    }

    [Test]
    public void finish_run_writes_the_record_and_clears_ongoing_game()
    {
        var store = new FakeSaveDataStore(PlayerSaveData.CreateEmpty());
        store.Dispatch(s => s.UpdateCurrentRunInfo(r => r with
        {
            OngoingGame = new OngoingGame(SampleGridSnapshot(), SampleStartInfo),
        }));
        var recorder = new GameRunRecorder(store);

        var sampleRunRecord = GameRunRecord.FromRun(SampleStartInfo, SampleStartInfo.StartAt.AddHours(1), true, BombMatrix);
        recorder.FinishRun(sampleRunRecord);

        store.State.History.Records.Should().Contain(sampleRunRecord);
        store.State.CurrentRunInfo.OngoingGame.Should().BeNull();
    }

    static readonly RunStartInfo SampleStartInfo = new(DateTime.UnixEpoch, new GridIndex(1, 2));

    static GridSnapshot SampleGridSnapshot() => new(
        new[,]
        {
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
        },
        BombMatrix);

    static readonly bool[,] BombMatrix = new[,] { { false, false }, { false, false } };
}

sealed class FakeSaveDataStore(PlayerSaveData initialState) : ISaveDataStore
{
    public PlayerSaveData State { get; private set; } = initialState;

    public void Dispatch(Func<PlayerSaveData, PlayerSaveData> reduce) => State = reduce(State);

    public event Action? SavedFeedback;

    public void NotifySaved() => SavedFeedback?.Invoke();
}