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

public sealed class GoldenEnvelopeSpecs
{
    static string FixtureFileName => $"playerSaveData.v{SaveVersion.Current}.json";
    const string V1SampleFileName = "playerSaveData.v1.json";
    const string RegenEnvVar = "SNEK_REGEN_GOLDEN";

    string _saveDir = null!;

    [Before(Test)]
    public void CreateTempDir()
    {
        _saveDir = Path.Combine(Path.GetTempPath(), $"SnekSweeper_GoldenSpecs_{Guid.NewGuid():N}");
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

    /// <summary>
    /// Pins the exact on-disk JSON envelope. Any save-schema change (field added/renamed/removed,
    /// type change, enum value change, bool[,] → bool[][] ...) fails this test until the golden
    /// fixture is intentionally regenerated and its diff reviewed.
    /// </summary>
    [Test]
    public void json_envelope_matches_golden()
    {
        CanonicalSaveSample().Save(_saveDir);

        var actual = NormalizeNewlines(File.ReadAllText(Path.Combine(_saveDir, SaveFileNames.Json)));
        var expected = NormalizeNewlines(File.ReadAllText(FixtureOutputPath));

        actual.Should().Be(expected);
    }

    [Test]
    public void v1_save_migrates_to_current()
    {
        var v1SamplePath = Path.Combine(AppContext.BaseDirectory, "SaveLoad", "Fixtures", V1SampleFileName);
        File.Copy(v1SamplePath, Path.Combine(_saveDir, SaveFileNames.Json));

        var loaded = PlayerSaveData.Load(_saveDir);

        loaded.Should().NotBeNull();
        loaded.Should().BeEquivalentTo(CanonicalSaveSample());
    }

    /// <summary>
    /// Regenerates the golden fixture from the current serializer output.
    /// Run only when a save-format change is intentional, then review the git diff of the fixture:
    ///   $env:SNEK_REGEN_GOLDEN="1"; dotnet run --project tests/BasicTests -- --treenode-filter "/*/*/GoldenEnvelopeSpecs/regenerate_golden"
    /// </summary>
    [Test]
    public void regenerate_golden()
    {
        if (Environment.GetEnvironmentVariable(RegenEnvVar) != "1") return;

        CanonicalSaveSample().Save(_saveDir);
        File.WriteAllText(FixtureSourcePath,
            File.ReadAllText(Path.Combine(_saveDir, SaveFileNames.Json)));
    }

    static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").TrimEnd('\n');

    static string FixtureOutputPath =>
        Path.Combine(AppContext.BaseDirectory, "SaveLoad", "Fixtures", FixtureFileName);

    static string FixtureSourcePath
    {
        get
        {
            var baseDir = Path.GetFullPath(AppContext.BaseDirectory);
            var current = new DirectoryInfo(baseDir);
            while (current is not null)
            {
                var fixturesDir = Path.Combine(current.FullName, "SaveLoad", "Fixtures");
                if (Directory.Exists(fixturesDir) && !IsUnder(baseDir, fixturesDir))
                {
                    return Path.Combine(fixturesDir, FixtureFileName);
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Could not locate the Fixtures source directory.");
        }
    }

    static bool IsUnder(string basePath, string candidate) =>
        Path.GetFullPath(candidate).StartsWith(Path.GetFullPath(basePath), StringComparison.OrdinalIgnoreCase);

    static PlayerSaveData CanonicalSaveSample() => new(
        new MainSetting
        {
            CurrentDifficultyKey = GridDifficultyKey.Expert,
            CurrentSkinKey = SkinKey.Mahjong,
            ComboRankDisplay = false,
            GenerateSolvableGrid = false,
        },
        new ActivatedCheatCodeSet([CheatCodeKey.Messenger]),
        new CurrentRunInfo
        {
            GridSnapshot = new GridSnapshot(
                [[CellSnapshotState.Revealed, CellSnapshotState.Flagged],
                 [CellSnapshotState.Covered, CellSnapshotState.Irrelevant]],
                new[,] { { false, true }, { true, false } }),
            StartInfo = new RunStartInfo(DateTime.UnixEpoch, new(1, 2)),
        },
        new History([WinningRecord()]));

    static GameRunRecord WinningRecord() => new(
        new RunDuration(DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
        true,
        new[,] { { false, true }, { true, false } },
        new(0, 0));
}
