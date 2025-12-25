using SnekSweeper.CellSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GameMode;

public class Referee(History history, Action onGameWin, Action onGameLose)
{
    DateTime _currentRunStartAt = DateTime.MinValue;

    internal void MarkRunStartTime() => _currentRunStartAt = DateTime.Now;

    internal void JudgeGame(Grid grid, List<Cell> bombCellsRevealed)
    {
        var result = GetGameResult(grid, bombCellsRevealed);
        HandleGameResult(result);
        return;

        static JudgedResult GetGameResult(Grid grid, List<Cell> bombCellsRevealed)
        {
            if (bombCellsRevealed.Count > 0)
            {
                return JudgedResult.Lose;
            }

            return grid.IsResolved ? JudgedResult.Win : JudgedResult.NothingHappens;
        }

        void HandleGameResult(JudgedResult gameResult)
        {
            switch (gameResult)
            {
                case JudgedResult.Win:
                    HandleGameWin(grid);
                    break;
                case JudgedResult.Lose:
                    HandleGameLose(grid, bombCellsRevealed);
                    break;
                case JudgedResult.NothingHappens:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameResult), gameResult, null);
            }
        }
    }

    void HandleGameLose(Grid grid, List<Cell> bombsRevealed)
    {
        SaveNewRecord(false, grid.BombMatrix, _currentRunStartAt, history);
        onGameLose();
    }

    void HandleGameWin(Grid grid)
    {
        SaveNewRecord(true, grid.BombMatrix, _currentRunStartAt, history);
        onGameWin();
    }

    static void SaveNewRecord(bool winning, bool[,] bombMatrix, DateTime runStartAt, History history)
    {
        var record = GameRunRecord.Create(
            RunDuration.Create(runStartAt, DateTime.Now),
            winning,
            bombMatrix
        );
        history.AddRecord(record);
    }

    enum JudgedResult
    {
        Win,
        Lose,
        NothingHappens,
    }
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