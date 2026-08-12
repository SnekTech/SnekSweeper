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
    /// <summary>
    /// Godot 层实例化一个格子：创建 humble cell、持有其 CellLogic，并装配出领域 <see cref="Cell"/>。
    /// </summary>
    Cell InstantiateCell(GridIndex gridIndex, GridSkin gridSkin);
    IEnumerable<IHumbleCell> HumbleCells { get; }
    void Clear();
}

public interface IGridCursor
{
    void ShowAt(GridIndex gridIndex, GridSize gridSize);
    void LockTo(GridIndex gridIndex, GridSize gridSize);
    void Unlock();
}
