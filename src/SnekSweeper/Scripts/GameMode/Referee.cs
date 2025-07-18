using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GameHistory;
using SnekSweeper.GridSystem;
using SnekSweeper.UI.GameResult;
using Widgets.Roguelike;

namespace SnekSweeper.GameMode;

public class Referee
{
    private readonly Grid _grid;
    private DateTime _currentRunStartAt = DateTime.MinValue;

    public Referee(Grid grid)
    {
        _grid = grid;
        _grid.BatchRevealed += OnBatchRevealed;
        _grid.BombRevealed += OnBombRevealed;
        _grid.InitCompleted += OnGridInitCompleted;
    }

    public void OnDispose()
    {
        _grid.BatchRevealed -= OnBatchRevealed;
        _grid.BombRevealed -= OnBombRevealed;
        _grid.InitCompleted -= OnGridInitCompleted;
    }

    private void SaveNewRecord(bool winning)
    {
        var (seed, state) = Rand.Data;
        var record = new GameRunRecord
        {
            StartAt = _currentRunStartAt,
            EndAt = DateTime.Now,
            Winning = winning,
            RandomData = new RandomData(seed, state),
        };
        HouseKeeper.History.AddRecord(record);
    }

    private void OnBombRevealed(List<Cell> bombsRevealed)
    {
        MessageBox.Print("Game over! Bomb revealed!");
        SaveNewRecord(false);

        Autoload.SceneSwitcher.GotoScene<LosingPage>();
    }

    private void OnBatchRevealed()
    {
        if (_grid.IsResolved)
        {
            MessageBox.Print("You win!");
            SaveNewRecord(true);

            Autoload.SceneSwitcher.GotoScene<WinningPage>();
        }
    }

    private void OnGridInitCompleted() => _currentRunStartAt = DateTime.Now;
}