namespace SnekSweeperCore.GridSystem.Difficulty;

public record GridDifficulty(GridDifficultyKey Key, GridDifficultyData DifficultyData);

public static class GridDifficultyExtensions
{
    extension(GridDifficulty difficulty)
    {
        public string Name => difficulty.Key.ToString();
    }
}

public readonly record struct GridDifficultyData(GridSize Size, int BombCount);

public enum GridDifficultyKey
{
    Beginner,
    Intermediate,
    Expert,
    Test,
}
