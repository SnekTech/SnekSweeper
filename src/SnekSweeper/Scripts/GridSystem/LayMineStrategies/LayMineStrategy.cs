namespace SnekSweeper.GridSystem.LayMineStrategies;

public interface ILayMineStrategy
{
    bool[,] Generate(GridIndex startIndex);
}

public enum LayMineStrategyKey
{
    Classic,
    Solvable,
    Hardcoded,
}

public static class LayMineStrategyKeyExtension
{
    extension(LayMineStrategyKey key)
    {
        public ILayMineStrategy ToStrategy(GridDifficultyData difficultyData) =>
            key switch
            {
                LayMineStrategyKey.Solvable => new Solvable(difficultyData),
                LayMineStrategyKey.Classic => new Classic(difficultyData),
                _ => new Solvable(difficultyData),
            };
    }
}