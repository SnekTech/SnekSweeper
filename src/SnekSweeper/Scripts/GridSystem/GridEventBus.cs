using System;

namespace SnekSweeper.GridSystem;

public class GridEventBus
{
    public event Action<int>? FlaggedCellCountChanged;

    public void EmitFlaggedCellCountChanged(int coveredBombCount) =>
        FlaggedCellCountChanged?.Invoke(coveredBombCount);
}