using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeperCore.CellSystem;

/// <summary>
/// 一格的表现：把"事实 + 转移结果"渲染出来。渲染是幂等的，视图不做任何决策。
/// </summary>
public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void Render(CellInfo info, CellOutcome outcome);
}

/// <summary>棋盘要渲染某一格时的去处（Godot 层按索引分派到具体节点）。</summary>
public interface ICellRenderer
{
    void Render(CellInfo info, CellOutcome outcome);
}
