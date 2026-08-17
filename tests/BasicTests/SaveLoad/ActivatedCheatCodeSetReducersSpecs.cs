using AwesomeAssertions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// B3：ActivatedCheatCodeSet record 化后的领域级 reducer 测试。
/// 同时覆盖 lens 提升（正确领域、原状态不可变）与子状态 Add 语义。
/// </summary>
public sealed class ActivatedCheatCodeSetReducersSpecs
{
    [Test]
    public void update_activated_cheat_code_set_adds_key_and_keeps_original_untouched()
    {
        var original = PlayerSaveData.CreateEmpty();
        var next = original.UpdateActivatedCheatCodeSet(s => s.Add(CheatCodeKey.Messenger));

        next.ActivatedCheatCodeSet.Contains(CheatCodeKey.Messenger).Should().BeTrue();
        original.ActivatedCheatCodeSet.Contains(CheatCodeKey.Messenger).Should().BeFalse();
    }
}
