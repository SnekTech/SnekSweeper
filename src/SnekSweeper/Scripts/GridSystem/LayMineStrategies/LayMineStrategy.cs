namespace SnekSweeper.GridSystem.LayMineStrategies;

public interface ILayMineStrategy
{
    bool[,] Generate(GridIndex startIndex);
}

public enum LayMineStrategyName
{
    Classic,
    Solvable,
    Hardcoded,
}

public static class LayMineStrategyNameExtensions
{
    extension(LayMineStrategyName strategyName)
    {
        public ILayMineStrategy ToStrategy(GridDifficultyData difficultyData) =>
            strategyName switch
            {
                LayMineStrategyName.Solvable => new Solvable(difficultyData),
                LayMineStrategyName.Classic => new Classic(difficultyData),
                _ => new Solvable(difficultyData),
            };
    }
}