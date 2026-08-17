using AwesomeAssertions;
using SnekSweeperCore.SaveLoad;

namespace BasicTests.SaveLoad;

/// <summary>
/// B2：MainSetting record 化后的领域级 reducer 测试。
/// 同时覆盖 lens 提升（正确领域、原状态不可变）与子状态语义。
/// </summary>
public sealed class MainSettingReducersSpecs
{
    [Test]
    public void update_main_setting_lifts_combo_rank_display_and_keeps_original_untouched()
    {
        var original = PlayerSaveData.CreateEmpty();
        var next = original.UpdateMainSetting(m => m with { ComboRankDisplay = false });

        next.MainSetting.ComboRankDisplay.Should().BeFalse();
        original.MainSetting.ComboRankDisplay.Should().BeTrue();
    }
}
