using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeperCore.CellSystem;

/// <summary>
/// 一格的表现：把"事实 + 转移结果"渲染出来。渲染是幂等的，视图不做任何决策。
/// </summary>
public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void Render(CellOutcome outcome);
    void Paint(CellInfo info, CellState state);
}

