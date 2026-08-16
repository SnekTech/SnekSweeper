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
    string _saveDir = null!;

    [Before(Test)]
    public void CreateTempDir()
    {
        _saveDir = Path.Combine(Path.GetTempPath(), $"SnekSweeper_VersionedSaveLoadTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_saveDir);
    }

    [After(Test)]
    public void CleanupTempDir()
    {
        if (Directory.Exists(_saveDir))
        {
            Directory.Delete(_saveDir, recursive: true);
        }
    }

    [Test]
    public void versioned_json_round_trips()
    {
        var original = SamplePlayerSaveData();

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void versioned_binary_round_trips()
    {
        var original = SamplePlayerSaveData();

        original.Save(_saveDir, SaveFormat.MemoryPack);

        var loaded = PlayerSaveData.Load(_saveDir, SaveFormat.MemoryPack);
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
    public void migrate_v1_to_current_yields_current_version_with_values()
    {
        var v1 = SampleDtoV1();

        var current = SaveMigrations.MigrateToCurrent(v1);

        // 从 v1 搬运过来的字段：逐字段与来源比较，避免在样本里 hardcode 期望值。
        // （将来 v2 有新增/改名字段时，再补"新字段取默认值/映射值"的显式断言——那时没有 v1 对应项。）
        current.MainSetting.CurrentDifficultyKey.Should().Be(v1.MainSetting.CurrentDifficultyKey);
        current.MainSetting.CurrentSkinKey.Should().Be(v1.MainSetting.CurrentSkinKey);
        current.MainSetting.ComboRankDisplay.Should().Be(v1.MainSetting.ComboRankDisplay);
        current.MainSetting.GenerateSolvableGrid.Should().Be(v1.MainSetting.GenerateSolvableGrid);
        current.ActivatedCheatCodeSet.ActivatedSet.Should().BeEquivalentTo(v1.ActivatedCheatCodeSet.ActivatedSet);
        current.History.Records.Should().BeEquivalentTo(v1.History.Records);
    }

    [Test]
    public void migrate_v1_snapshot_states_to_2d()
    {
        var v1 = SampleDtoV1WithSnapshot();

        var v2 = SaveMigrations.MigrateV1ToV2(v1);

        var snapshot = v2.CurrentRunInfo.GridSnapshot;
        snapshot.Should().NotBeNull();
        snapshot.SnapshotStates.GetLength(0).Should().Be(2);
        snapshot.SnapshotStates.GetLength(1).Should().Be(2);
        snapshot.SnapshotStates[0, 0].Should().Be(CellSnapshotState.Revealed);
        snapshot.SnapshotStates[1, 1].Should().Be(CellSnapshotState.Irrelevant);
        snapshot.BombMatrix[1, 0].Should().BeTrue();
    }

    [Test]
    public void load_json_with_unsupported_version_returns_null()
    {
        File.WriteAllText(Path.Combine(_saveDir, SaveFileNames.Json),
            """{ "version": 99, "data": {} }""");

        PlayerSaveData.Load(_saveDir).Should().BeNull();
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
                new[,]
                {
                    { CellSnapshotState.Revealed, CellSnapshotState.Flagged },
                    { CellSnapshotState.Covered, CellSnapshotState.Irrelevant },
                },
                new[,] { { false, true }, { true, false } }),
            StartInfo = new RunStartInfo(DateTime.UnixEpoch, new(1, 2)),
        },
        new History([WinningRecord()]));

    static GameRunRecord WinningRecord() => new(
        new RunDuration(DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
        true,
        new[,] { { false, true }, { true, false } },
        new(0, 0));

    static PlayerSaveDataDtoV1 SampleDtoV1() => new(
        new MainSettingDtoV1(GridDifficultyKey.Expert, SkinKey.Mahjong, false, false),
        new ActivatedCheatCodeSetDtoV1([CheatCodeKey.Messenger]),
        new CurrentRunInfoDtoV1(null, new RunStartInfo(DateTime.UnixEpoch, new(1, 2))),
        new HistoryDtoV1([]));

    static PlayerSaveDataDtoV1 SampleDtoV1WithSnapshot() => new(
        new MainSettingDtoV1(GridDifficultyKey.Expert, SkinKey.Mahjong, false, false),
        new ActivatedCheatCodeSetDtoV1([CheatCodeKey.Messenger]),
        new CurrentRunInfoDtoV1(
            new GridSnapshotV1(
                [[CellSnapshotState.Revealed, CellSnapshotState.Flagged],
                 [CellSnapshotState.Covered, CellSnapshotState.Irrelevant]],
                new[,] { { false, true }, { true, false } }),
            new RunStartInfo(DateTime.UnixEpoch, new(1, 2))),
        new HistoryDtoV1([]));
}
