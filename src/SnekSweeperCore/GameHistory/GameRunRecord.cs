using MemoryPack;
using SnekSweeperCore.GridSystem;

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
        internal static GameRunRecord Create(RunDuration duration, bool winning, bool[,] bombMatrix,
            GridIndex startIndex)
            => new(duration, winning, bombMatrix, startIndex);
    }
}