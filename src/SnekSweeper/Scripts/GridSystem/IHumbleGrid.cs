using SnekSweeper.CellSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    List<IHumbleCell> InstantiateHumbleCells(int count);
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
}