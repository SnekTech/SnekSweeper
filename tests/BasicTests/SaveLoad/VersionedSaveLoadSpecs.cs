using AwesomeAssertions;
using SnekSweeperCore.CheatCodeSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameSettings;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SaveLoad;
using SnekSweeperCore.SkinSystem;

namespace BasicTests.SaveLoad;

public sealed class VersionedSaveLoadSpecs
{
    string SaveDir = null!;

    [Before(Test)]
    public void CreateTempDir()
    {
        SaveDir = Path.Combine(Path.GetTempPath(), $"SnekSweeper_VersionedSaveLoadTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(SaveDir);
    }

    [After(Test)]
    public void CleanupTempDir()
    {
        if (Directory.Exists(SaveDir))
        {
            Directory.Delete(SaveDir, recursive: true);
        }
    }

    [Test]
    public void versioned_json_round_trips()
    {
        var original = SamplePlayerSaveData();

        original.Save(SaveDir);

        var loaded = PlayerSaveData.Load(SaveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void versioned_binary_round_trips()
    {
        var original = SamplePlayerSaveData();

        original.Save(SaveDir, SaveFormat.MemoryPack);

        var loaded = PlayerSaveData.Load(SaveDir, SaveFormat.MemoryPack);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void migrate_v1_to_v2_copies_each_field()
    {
        var v1 = SampleDtoV1();

        var v2 = SaveMigrations.MigrateV1ToV2(v1);

        v2.MainSetting.CurrentDifficultyKey.Should().Be(v1.MainSetting.CurrentDifficultyKey);
        v2.MainSetting.CurrentSkinKey.Should().Be(v1.MainSetting.CurrentSkinKey);
        v2.MainSetting.ComboRankDisplay.Should().Be(v1.MainSetting.ComboRankDisplay);
        v2.MainSetting.GenerateSolvableGrid.Should().Be(v1.MainSetting.GenerateSolvableGrid);
        v2.ActivatedCheatCodeSet.ActivatedSet.Should().BeEquivalentTo(v1.ActivatedCheatCodeSet.ActivatedSet);
        v2.CurrentRunInfo.Should().BeEquivalentTo(v1.CurrentRunInfo);
        v2.History.Records.Should().BeEmpty();
    }

    [Test]
    public void migrate_v2_to_v3_copies_each_field()
    {
        var v2 = SampleDtoV2();

        var v3 = SaveMigrations.MigrateV2ToV3(v2);

        v3.MainSetting.CurrentDifficultyKey.Should().Be(v2.MainSetting.CurrentDifficultyKey);
        v3.MainSetting.CurrentSkinKey.Should().Be(v2.MainSetting.CurrentSkinKey);
        v3.MainSetting.ComboRankDisplay.Should().Be(v2.MainSetting.ComboRankDisplay);
        v3.MainSetting.GenerateSolvableGrid.Should().Be(v2.MainSetting.GenerateSolvableGrid);
        v3.ActivatedCheatCodeSet.ActivatedSet.Should().BeEquivalentTo(v2.ActivatedCheatCodeSet.ActivatedSet);
        v3.CurrentRunInfo.Should().BeEquivalentTo(v2.CurrentRunInfo);
        v3.History.Records.Should().BeEmpty();
    }

    [Test]
    public void migrate_v1_to_current_yields_current_version_with_values()
    {
        var v1 = SampleDtoV1();

        var current = SaveMigrations.MigrateToCurrent(v1);

        // 迁移结果必须是"当前版本 DTO 且携带正确的迁移后值"。
        // 真实 schema 变化时这里断言的是：旧字段搬运过来、新字段取默认值、改名/改类型字段映射正确。
        current.MainSetting.CurrentDifficultyKey.Should().Be(GridDifficultyKey.Expert);
        current.MainSetting.CurrentSkinKey.Should().Be(SkinKey.Mahjong);
        current.MainSetting.ComboRankDisplay.Should().BeFalse();
        current.MainSetting.GenerateSolvableGrid.Should().BeFalse();
        current.ActivatedCheatCodeSet.ActivatedSet.Should().BeEquivalentTo([CheatCodeKey.Messenger]);
        current.History.Records.Should().BeEmpty();
    }

    [Test]
    public void load_json_with_unsupported_version_returns_null()
    {
        File.WriteAllText(Path.Combine(SaveDir, SaveFileNames.Json),
            """{ "version": 99, "data": {} }""");

        PlayerSaveData.Load(SaveDir).Should().BeNull();
    }

    static PlayerSaveData SamplePlayerSaveData() => new(
        new MainSetting
        {
            CurrentDifficultyKey = GridDifficultyKey.Expert,
            CurrentSkinKey = SkinKey.Mahjong,
            ComboRankDisplay = false,
            GenerateSolvableGrid = false,
        },
        new ActivatedCheatCodeSet([CheatCodeKey.TransparentCover, CheatCodeKey.Messenger]),
        new CurrentRunInfo
        {
            GridSnapshot = new GridSnapshot(
                new[]
                {
                    new[] { CellSnapshotState.Revealed, CellSnapshotState.Flagged },
                    new[] { CellSnapshotState.Covered, CellSnapshotState.Irrelevant },
                },
                new[,] { { false, true }, { true, false } }),
            StartInfo = new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2)),
        },
        new History([WinningRecord()]));

    static GameRunRecord WinningRecord() => new(
        new RunDuration(DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
        true,
        new[,] { { false, true }, { true, false } },
        new GridIndex(0, 0));

    static PlayerSaveDataDtoV1 SampleDtoV1() => new(
        new MainSettingDtoV1(GridDifficultyKey.Expert, SkinKey.Mahjong, false, false),
        new ActivatedCheatCodeSetDtoV1([CheatCodeKey.Messenger]),
        new CurrentRunInfoDtoV1(null, new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2))),
        new HistoryDtoV1([]));

    static PlayerSaveDataDtoV2 SampleDtoV2() => new(
        new MainSettingDtoV2(GridDifficultyKey.Expert, SkinKey.Mahjong, false, false),
        new ActivatedCheatCodeSetDtoV2([CheatCodeKey.Messenger]),
        new CurrentRunInfoDtoV2(null, new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2))),
        new HistoryDtoV2([]));
}
