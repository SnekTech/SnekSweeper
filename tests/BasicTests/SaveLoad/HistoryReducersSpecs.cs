using AwesomeAssertions;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// B4：History record 化后的领域级 reducer 测试。
/// 同时覆盖 lens 提升（正确领域、原状态不可变）与子状态 Add 语义。
/// </summary>
public sealed class HistoryReducersSpecs
{
    [Test]
    public void update_history_adds_record_and_keeps_original_untouched()
    {
        var original = PlayerSaveData.CreateEmpty();
        var record = WinningRecord();

        var next = original.UpdateHistory(h => h.Add(record));

        next.History.Records.Should().Contain(record);
        original.History.Records.Should().BeEmpty();
    }

    static GameRunRecord WinningRecord() => new(
        new RunDuration(DateTime.Now.AddMinutes(-12), DateTime.Now.AddMinutes(-11)),
        true,
        new[,]
        {
            { false, false, true },
            { false, true, false },
            { true, false, false },
        },
        new(0, 0));
}
