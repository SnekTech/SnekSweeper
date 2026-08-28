using MemoryPack;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GameHistory;

public readonly record struct RunDuration(DateTime StartAt, DateTime EndAt);

[MemoryPackable]
public partial record GameRunRecord(
    RunDuration Duration,
    bool Winning,
    bool[,] BombMatrix,
    GridIndex StartIndex);

static class GameRunRecordExtensions
{
    extension(RunDuration)
    {
        internal static RunDuration Create(DateTime startAt, DateTime endAt) => new(startAt, endAt);
    }

    extension(GameRunRecord)
    {
        internal static GameRunRecord FromRun(RunStartInfo startInfo, DateTime endAt, bool winning, bool[,] bombMatrix) =>
            new(RunDuration.Create(startInfo.StartAt, endAt), winning, bombMatrix, startInfo.StartIndex);
    }
}
