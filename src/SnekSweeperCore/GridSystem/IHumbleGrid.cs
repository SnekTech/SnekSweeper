using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem.CursorManagement;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    IHumbleCellCollection HumbleCellsContainer { get; }
    void ApplyGridOutcome(GridOutcome gridOutcome);
    void Paint(Grid grid);
    void TriggerInitEffects();
    IGridCursor GridCursor { get; }
    void PlayCongratulationEffects();
}

/// <summary>
/// 集合角色：暴露 / 清空 humble cell 集合（Godot 层实现）。
/// </summary>
public interface IHumbleCellCollection
{
    IEnumerable<IHumbleCell> HumbleCells { get; }
    IHumbleCell CellAt(GridIndex index);
    void Clear();
}

public interface IGridCursor
{
    /// <summary>把光标移到指针位置：指向某一格就显示在那里，不在网格内就隐藏。</summary>
    void ShowAt(PointerTarget target);
    void SetCursorPolicy(CursorPolicy policy);
}
