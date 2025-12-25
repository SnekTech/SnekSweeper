using SnekSweeper.Commands;
using SnekSweeper.GameMode;
using SnekSweeperCore.CellSystem;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
}