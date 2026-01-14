using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.CellSystem;

public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void OnInstantiate(GridIndex gridIndex, GridSkin skin);
    void OnInit(CellInitData initData);
}

public readonly record struct CellInitData(bool HasBomb, int NeighborBombCount);