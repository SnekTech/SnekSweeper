using SnekSweeperCore.CellSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    IHumbleCellCollection HumbleCellsContainer { get; }
    void TriggerInitEffects();
    IGridCursor GridCursor { get; }
    void PlayCongratulationEffects();
}

/// <summary>
/// 工厂角色：为 Core 的 Grid.Create 创建领域 <see cref="Cell"/>（Godot 层实现）。
/// </summary>
public interface ICellFactory
{
    Cell InstantiateCell(GridIndex gridIndex, GridSkin gridSkin);
}

/// <summary>
/// 集合角色：暴露 / 清空 humble cell 集合（Godot 层实现）。
/// </summary>
public interface IHumbleCellCollection
{
    IEnumerable<IHumbleCell> HumbleCells { get; }
    void Clear();
}

public interface IGridCursor
{
    void ShowAt(GridIndex gridIndex, GridSize gridSize);
    void LockTo(GridIndex gridIndex, GridSize gridSize);
    void Unlock();
}
