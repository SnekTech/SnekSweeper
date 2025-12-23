using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;

namespace SnekSweeper.GridSystem;

interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IHumbleCell[,] InstantiateHumbleCells(GridSize gridSize);
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
}