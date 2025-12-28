using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
}