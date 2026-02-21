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

        if (cellsInThisBatch.Any(cell => cell.IsRevealedBomb))
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

public sealed record GameLose(bool[,] Bombs, List<Cell> CellsInThisBatch) : JudgedResult;

public sealed record Surviving : JudgedResult
{
    public static Surviving Instance { get; } = new();
}

static class GridExtensionsForReferee
{
    extension(Grid grid)
    {
        // if all safe cells are revealed, the grid is resolved
        internal bool IsResolved => grid.Cells.Where(cell => !cell.HasBomb).All(cell => cell.IsRevealed);

        internal bool[,] BombMatrix
        {
            get
            {
                var size = grid.Size;
                var bombs = new bool[size.Rows, size.Columns];
                foreach (var cell in grid.Cells)
                {
                    bombs.SetAt(cell.GridIndex, cell.HasBomb);
                }

                return bombs;
            }
        }
    }
}