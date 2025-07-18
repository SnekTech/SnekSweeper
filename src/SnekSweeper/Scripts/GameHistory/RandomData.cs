namespace SnekSweeper.GameHistory;

public record struct RandomData(ulong Seed, ulong State)
{
    public static RandomData Empty => new(0, 0);
}