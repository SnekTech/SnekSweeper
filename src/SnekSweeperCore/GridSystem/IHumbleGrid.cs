using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IEnumerable<IHumbleCell> HumbleCells { get; }
    void TriggerInitEffects();
    IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin);
    IGridCursor GridCursor { get; }
}

public interface IGridCursor
{
    void ShowAt(GridIndex gridIndex, GridSize gridSize);
    void LockTo(GridIndex gridIndex, GridSize gridSize);
    void Unlock();
}