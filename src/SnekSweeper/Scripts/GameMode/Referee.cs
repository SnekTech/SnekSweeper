using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;
using SnekSweeper.UI.GameResult;

namespace SnekSweeper.GameMode;

public class Referee
{
    readonly Grid _grid;
    DateTime _currentRunStartAt = DateTime.MinValue;
    readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;

    public Referee(Grid grid)
    {
        _grid = grid;
        _gridEventBus.BatchRevealed += OnBatchRevealed;
        _gridEventBus.BombRevealed += OnBombRevealed;
        _gridEventBus.InitCompleted += OnGridInitCompleted;
    }

    public void Dispose()
    {
        _gridEventBus.BatchRevealed -= OnBatchRevealed;
        _gridEventBus.BombRevealed -= OnBombRevealed;
        _gridEventBus.InitCompleted -= OnGridInitCompleted;
    }

    void SaveNewRecord(bool winning)
    {
        var record = GameRunRecord.Create(
            RunDuration.Create(_currentRunStartAt, DateTime.Now),
            winning,
            _grid.BombMatrix
        );
        HouseKeeper.History.AddRecord(record);
    }

    void OnBombRevealed(List<Cell> bombsRevealed)
    {
        MessageBox.Print("Game over! Bomb revealed!");
        SaveNewRecord(false);

        Autoload.SceneSwitcher.GotoScene<LosingPage>();
    }

    void OnBatchRevealed()
    {
        if (_grid.IsResolved)
        {
            MessageBox.Print("You win!");
            SaveNewRecord(true);

            Autoload.SceneSwitcher.GotoScene<WinningPage>();
        }
    }

    void OnGridInitCompleted() => _currentRunStartAt = DateTime.Now;
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