using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.GameMode;

public static class Referee
{
    public static JudgedResult Judge(GridInputProcessResult processResult) =>
        processResult switch
        {
            BatchRevealed batchRevealed => GetGameResult(batchRevealed),
            _ => Surviving.Instance,
        };

    static JudgedResult GetGameResult(BatchRevealed batchRevealed)
    {
        var (grid, cellsInThisBatch) = batchRevealed;

        if (cellsInThisBatch.Any(index => grid.StateAt(index).IsRevealed && grid.InfoAt(index).HasBomb))
        {
            return new GameLose(grid.BombMatrix, cellsInThisBatch);
        }

        if (grid.IsResolved)
        {
            return new GameWin(grid.BombMatrix);
        }

        return Surviving.Instance;
    }
}

public abstract record JudgedResult;

public sealed record GameWin(bool[,] Bombs) : JudgedResult;

public sealed record GameLose(bool[,] Bombs, IReadOnlyList<GridIndex> CellsInThisBatch) : JudgedResult;

public sealed record Surviving : JudgedResult
{
    public static Surviving Instance { get; } = new();
}
