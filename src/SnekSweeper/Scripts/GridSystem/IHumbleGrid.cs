using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;

namespace SnekSweeper.GridSystem;

interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
}