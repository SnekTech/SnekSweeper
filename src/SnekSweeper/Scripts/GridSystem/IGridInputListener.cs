using System;

namespace SnekSweeper.GridSystem;

public interface IGridInputListener
{
    public event Action<GridIndex>? PrimaryReleased;
    public event Action<GridIndex>? PrimaryDoubleClicked;
    public event Action<GridIndex>? SecondaryReleased;
}