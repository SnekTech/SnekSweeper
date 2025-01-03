using SnekSweeper.CellSystem.Components;
using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void SetContent(bool hasBomb, int neighborBombCount);
    void SetPosition(GridIndex gridIndex);
}