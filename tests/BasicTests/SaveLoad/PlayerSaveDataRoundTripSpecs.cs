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

public sealed class PlayerSaveDataRoundTripSpecs
{
    string _saveDir = null!;

    [Before(Test)]
    public void CreateTempDir()
    {
        _saveDir = Path.Combine(Path.GetTempPath(), $"SnekSweeper_SaveLoadTests_{Guid.NewGuid():N}");
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
    public void empty_save_round_trips()
    {
        var original = PlayerSaveData.CreateEmpty();

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void customized_settings_round_trip()
    {
        var original = new PlayerSaveData(
            CustomizedMainSetting(),
            new ActivatedCheatCodeSet([]),
            new CurrentRunInfo(),
            new History([]));

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void activated_cheat_codes_round_trip()
    {
        var original = new PlayerSaveData(
            new MainSetting(),
            new ActivatedCheatCodeSet([CheatCodeKey.TransparentCover, CheatCodeKey.Messenger]),
            new CurrentRunInfo(),
            new History([]));

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void unfinished_run_snapshot_round_trip()
    {
        var original = new PlayerSaveData(
            new MainSetting(),
            new ActivatedCheatCodeSet([]),
            new CurrentRunInfo
            {
                GridSnapshot = SampleGridSnapshot(),
                StartInfo = SampleStartInfo(),
            },
            new History([]));

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void history_records_round_trip()
    {
        var original = new PlayerSaveData(
            new MainSetting(),
            new ActivatedCheatCodeSet([]),
            new CurrentRunInfo(),
            new History([WinningRecord(), LosingRecord()]));

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public void fully_populated_save_round_trips()
    {
        var original = new PlayerSaveData(
            CustomizedMainSetting(),
            new ActivatedCheatCodeSet([CheatCodeKey.TransparentCover, CheatCodeKey.Messenger]),
            new CurrentRunInfo
            {
                GridSnapshot = SampleGridSnapshot(),
                StartInfo = SampleStartInfo(),
            },
            new History([WinningRecord(), LosingRecord()]));

        original.Save(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public async Task async_save_round_trips()
    {
        var original = new PlayerSaveData(
            CustomizedMainSetting(),
            new ActivatedCheatCodeSet([CheatCodeKey.Messenger]),
            new CurrentRunInfo
            {
                GridSnapshot = SampleGridSnapshot(),
                StartInfo = SampleStartInfo(),
            },
            new History([WinningRecord()]));

        await original.SaveAsync(_saveDir);

        var loaded = PlayerSaveData.Load(_saveDir);
        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(original);
    }

    [Test]
    public async Task async_save_leaves_no_temp_files()
    {
        var original = PlayerSaveData.CreateEmpty();

        await original.SaveAsync(_saveDir);

        var files = Directory.GetFiles(_saveDir).Select(Path.GetFileName);
        files.Should().BeEquivalentTo(["playerSaveData.json"]);
    }

    [Test]
    public void load_returns_null_when_no_save_exists()
    {
        var loaded = PlayerSaveData.Load(_saveDir);

        loaded.Should().BeNull();
    }

    static MainSetting CustomizedMainSetting() => new()
    {
        CurrentDifficultyKey = GridDifficultyKey.Expert,
        CurrentSkinKey = SkinKey.Mahjong,
        ComboRankDisplay = false,
        GenerateSolvableGrid = false,
    };

    static GridSnapshot SampleGridSnapshot() => new(
        [[CellSnapshotState.Revealed, CellSnapshotState.Flagged, CellSnapshotState.Covered],
         [CellSnapshotState.Irrelevant, CellSnapshotState.Revealed, CellSnapshotState.Flagged],
         [CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Revealed]],
        new[,]
        {
            { false, false, true },
            { false, true, false },
            { true, false, false },
        });

    static RunStartInfo SampleStartInfo() => new(DateTime.Now.AddMinutes(-8), new(1, 1));

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

    static GameRunRecord LosingRecord() => new(
        new RunDuration(DateTime.Now.AddMinutes(-30), DateTime.Now.AddMinutes(-29)),
        false,
        new[,]
        {
            { true, false },
            { false, false },
        },
        new(0, 1));
}
