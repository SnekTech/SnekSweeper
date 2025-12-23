using SnekSweeper.CellSystem.Components;

namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void SetContent(bool hasBomb, int neighborBombCount);
}