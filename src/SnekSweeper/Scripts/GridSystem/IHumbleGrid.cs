using System;
using System.Collections.Generic;
using SnekSweeper.CellSystem;

namespace SnekSweeper.GridSystem;

public interface IHumbleGrid
{
    public event Action<(int i, int j)>? PrimaryReleased;
    public event Action<(int i, int j)>? PrimaryDoubleClicked;
    public event Action<(int i, int j)>? SecondaryReleased;
    List<IHumbleCell> InstantiateHumbleCells(int count);
}