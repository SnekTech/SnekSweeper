using System.Collections.Generic;
using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    List<IHumbleCell> InstantiateHumbleCells(int count);
}