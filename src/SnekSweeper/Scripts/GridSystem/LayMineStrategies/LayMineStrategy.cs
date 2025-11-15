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