namespace SnekSweeperCore.GridSystem.LayMineStrategies;

public sealed class Hardcoded(bool[,] hardcodedBombs) : ILayMineStrategy
{
    public bool[,] Generate(GridIndex startIndex) => hardcodedBombs;
}