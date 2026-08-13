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
    public void migrate_current_version_preserves_data()
    {
        var dto = SampleDto();

        SaveMigrations.MigrateToCurrent(dto, SaveVersion.Current).Should().BeEquivalentTo(dto);
    }

    [Test]
    public void migrate_unsupported_version_throws()
    {
        var dto = SampleDto();

        Action migrate = () => SaveMigrations.MigrateToCurrent(dto, 99);

        migrate.Should().Throw<SaveVersionNotSupportedException>();
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

    static PlayerSaveDataDto SampleDto() => new(
        new MainSettingDto(GridDifficultyKey.Expert, SkinKey.Mahjong, false, false),
        new ActivatedCheatCodeSetDto([CheatCodeKey.Messenger]),
        new CurrentRunInfoDto(null, new RunStartInfo(DateTime.UnixEpoch, new GridIndex(1, 2))),
        new HistoryDto([]));
}
