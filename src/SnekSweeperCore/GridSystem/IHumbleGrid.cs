using SnekSweeperCore.CellSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    IHumbleCellsContainer HumbleCellsContainer { get; }
    void TriggerInitEffects();
    IGridCursor GridCursor { get; }
    void PlayCongratulationEffects();
}

public interface IHumbleCellsContainer
{
    // todo: may have a better design
    CellInstance InstantiateCell(GridIndex gridIndex, GridSkin gridSkin);
    IEnumerable<IHumbleCell> HumbleCells { get; }
    void Clear();
}

public interface IGridCursor
{
    void ShowAt(GridIndex gridIndex, GridSize gridSize);
    void LockTo(GridIndex gridIndex, GridSize gridSize);
    void Unlock();
}
