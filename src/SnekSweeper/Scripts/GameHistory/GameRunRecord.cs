namespace SnekSweeper.GameHistory;

public class GameRunRecord
{
    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }
    public bool Winning { get; init; }
    public RandomData RandomData { get; init; }
}