using SnekSweeperCore.CellSystem.Components;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.CellSystem;

public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void SetPosition(GridIndex gridIndex);
    void SetSkin(GridSkin skin);
    void SetContent(bool hasBomb, int neighborBombCount);
}