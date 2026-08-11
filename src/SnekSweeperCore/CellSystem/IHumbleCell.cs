using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.CellSystem;

public interface IHumbleCell
{
    // Cover/Flag 仍被外部通过接口调用（cheat code 的 SetAlpha、tutorial 的 SetStatus），保留。
    // MarkAsWrongFlagged / MarkAsBombRevealed 已不再被 Core 调用，改为 HumbleCell 内部方法。
    ICover Cover { get; }
    IFlag Flag { get; }
    void OnInstantiate(GridIndex gridIndex, GridSkin skin);
    void OnInit(CellInitData initData);
}

public readonly record struct CellInitData(bool HasBomb, int NeighborBombCount);
