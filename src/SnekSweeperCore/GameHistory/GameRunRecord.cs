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

public static class GameRunRecordExtensions
{
    extension(GameRunRecord)
    {
        public static GameRunRecord FromRun(RunStartInfo startInfo, DateTime endAt, bool winning, bool[,] bombMatrix) =>
            new(new RunDuration(startInfo.StartAt, endAt), winning, bombMatrix, startInfo.StartIndex);
    }
}
