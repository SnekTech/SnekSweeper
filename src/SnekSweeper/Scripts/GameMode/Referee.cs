using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;
using SnekSweeperCore.CellSystem;
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
    }

    void HandleGameResult(JudgedResult gameResult)
    {
        switch (gameResult)
        {
            case GameWin winResult:
                HandleGameWin(winResult.Record);
                break;
            case GameLose loseResult:
                HandleGameLose(loseResult.Record);
                break;
            case NothingHappens:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameResult), gameResult, null);
        }
    }

    JudgedResult GetGameResult(Grid grid, List<Cell> bombCellsRevealed)
    {
        if (bombCellsRevealed.Count > 0)
        {
            var loseRecord = CreateRecord(false);
            return new GameLose(loseRecord);
        }

        if (grid.IsResolved)
        {
            var winRecord = CreateRecord(true);
            return new GameWin(winRecord);
        }

        return new NothingHappens();

        GameRunRecord CreateRecord(bool winning) =>
            GameRunRecord.Create(
                RunDuration.Create(_currentRunStartAt, DateTime.Now),
                winning,
                grid.BombMatrix
            );
    }

    void HandleGameLose(GameRunRecord loseRecord)
    {
        history.AddRecord(loseRecord);
        onGameLose();
    }

    void HandleGameWin(GameRunRecord winRecord)
    {
        history.AddRecord(winRecord);
        onGameWin();
    }

    abstract record JudgedResult;

    sealed record GameWin(GameRunRecord Record) : JudgedResult;

    sealed record GameLose(GameRunRecord Record) : JudgedResult;

    sealed record NothingHappens : JudgedResult;
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