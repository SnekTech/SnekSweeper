using System.Collections.Generic;
using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    IGridInputListener GridInputListener { get; }
    List<IHumbleCell> InstantiateHumbleCells(int count);
}