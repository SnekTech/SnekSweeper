using AwesomeAssertions;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// CurrentRunInfo 改为 OngoingGame? 后的领域级 reducer 测试。
/// 覆盖 lens 提升（正确领域、原状态不可变）与 OngoingGame 语义。
/// </summary>
public sealed class CurrentRunInfoReducersSpecs
{
    [Test]
    public void update_current_run_info_lifts_ongoing_game_and_keeps_original_untouched()
    {
        var original = PlayerSaveData.CreateEmpty();
        var ongoingGame = new OngoingGame(
            SampleGridSnapshot(),
            new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2)));

        var next = original.UpdateCurrentRunInfo(r => r with { OngoingGame = ongoingGame });

        next.CurrentRunInfo.OngoingGame.Should().Be(ongoingGame);
        original.CurrentRunInfo.OngoingGame.Should().BeNull();
    }

    static GridSnapshot SampleGridSnapshot() => new(
        new[,]
        {
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
            { CellSnapshotState.Covered, CellSnapshotState.Covered },
        },
        new[,] { { false, false }, { false, false } });
}
