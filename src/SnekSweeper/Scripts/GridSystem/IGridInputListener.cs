using System;

namespace SnekSweeper.GridSystem;

public interface IGridInputListener
{
    public event Action<(int i, int j)>? PrimaryReleased;
    public event Action<(int i, int j)>? PrimaryDoubleClicked;
    public event Action<(int i, int j)>? SecondaryReleased;
}