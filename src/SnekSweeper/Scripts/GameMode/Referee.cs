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
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;

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

    private void SaveNewRecord(bool winning)
    {
        var (seed, state) = Rand.Data;
        var record = new GameRunRecord
        {
            StartAt = _currentRunStartAt,
            EndAt = DateTime.Now,
            Winning = winning,
            RngData = new RngData(seed, state),
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