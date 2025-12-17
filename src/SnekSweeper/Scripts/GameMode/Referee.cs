using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;
using SnekSweeper.UI.GameResult;

namespace SnekSweeper.GameMode;

public class Referee
{
    DateTime _currentRunStartAt = DateTime.MinValue;

    internal void MarkRunStartTime() => _currentRunStartAt = DateTime.Now;

    internal void HandleGameLose(Grid grid, List<Cell> bombsRevealed)
    {
        MessageBox.Print("Game over! Bomb revealed!");
        SaveNewRecord(false, grid.BombMatrix);

        Autoload.SceneSwitcher.GotoScene<LosingPage>();
    }

    internal void CheckIfGridResolved(Grid grid)
    {
        if (!grid.IsResolved) return;

        MessageBox.Print("You win!");
        SaveNewRecord(true, grid.BombMatrix);

        Autoload.SceneSwitcher.GotoScene<WinningPage>();
    }

    void SaveNewRecord(bool winning, bool[,] bombMatrix)
    {
        var record = GameRunRecord.Create(
            RunDuration.Create(_currentRunStartAt, DateTime.Now),
            winning,
            bombMatrix
        );
        HouseKeeper.History.AddRecord(record);
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