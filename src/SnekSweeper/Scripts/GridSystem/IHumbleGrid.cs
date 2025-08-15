using SnekSweeper.CellSystem;
using SnekSweeper.Commands;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    List<IHumbleCell> InstantiateHumbleCells(int count);
    IEnumerable<IHumbleCell> HumbleCells { get; }
}