using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    IHumbleCellsContainer HumbleCellsContainer { get; }
    CommandInvoker GridCommandInvoker { get; }
    void TriggerInitEffects();
    IGridCursor GridCursor { get; }
    void PlayCongratulationEffects();
}

public interface IHumbleCellsContainer
{
    IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin);
    IEnumerable<IHumbleCell> HumbleCells { get; }
    void Clear();
}

public interface IGridCursor
{
    void ShowAt(GridIndex gridIndex, GridSize gridSize);
    void LockTo(GridIndex gridIndex, GridSize gridSize);
    void Unlock();
}