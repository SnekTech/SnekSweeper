using System;

namespace SnekSweeper.GridSystem;

public class GridEventBus
{
    public event Action<int>? FlagCountChanged;

    public void EmitFlagCountChanged(int coveredBombCount) =>
        FlagCountChanged?.Invoke(coveredBombCount);
}