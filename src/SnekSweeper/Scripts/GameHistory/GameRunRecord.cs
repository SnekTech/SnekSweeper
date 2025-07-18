using Widgets.Roguelike;

namespace SnekSweeper.GameHistory;

public class GameRunRecord
{
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
    public bool Winning { get; init; }
    public RngData RngData { get; init; }
}