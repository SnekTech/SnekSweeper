using AwesomeAssertions;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// B1：CurrentRunInfo record 化后的领域级 reducer 测试。
/// 同时覆盖 lens 提升（正确领域、原状态不可变）与子状态语义。
/// </summary>
public sealed class CurrentRunInfoReducersSpecs
{
    [Test]
    public void update_current_run_info_lifts_start_info_and_keeps_original_untouched()
    {
        var original = PlayerSaveData.CreateEmpty();
        var startInfo = new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2));

        var next = original.UpdateCurrentRunInfo(r => r with { StartInfo = startInfo });

        next.CurrentRunInfo.StartInfo.Should().Be(startInfo);
        original.CurrentRunInfo.StartInfo.Should().Be(default(RunStartInfo));
    }
}
