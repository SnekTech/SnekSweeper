using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.GameMode;

public static class Referee
{
    public static JudgedResult Judge(Grid grid, GridOutcome gridOutcome) =>
        gridOutcome switch
        {
            GridOutcome.BatchRevealed batchRevealed => GetGameResult(grid, batchRevealed),
            _ => Surviving.Instance,
        };

    static JudgedResult GetGameResult(Grid grid, GridOutcome.BatchRevealed batchRevealed)
    {
        var cells = batchRevealed.Cells;

        if (cells.Any(cellOutcome => grid.StateAt(cellOutcome.Info.Index).IsRevealed && cellOutcome.Info.HasBomb))
        {
            return new GameLose(grid.BombMatrix, [..cells.Select(c => c.Info.Index)]);
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
