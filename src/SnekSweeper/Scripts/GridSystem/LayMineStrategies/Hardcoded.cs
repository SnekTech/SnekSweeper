using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem.LayMineStrategies;

public sealed class Hardcoded(bool[,] hardcodedBombs) : ILayMineStrategy
{
    public bool[,] Generate(GridIndex startIndex) => hardcodedBombs;
}