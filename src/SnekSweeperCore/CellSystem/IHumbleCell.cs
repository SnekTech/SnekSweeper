using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeperCore.CellSystem;

public interface IHumbleCell
{
    ICover Cover { get; }
    IFlag Flag { get; }
    void SetContent(bool hasBomb, int neighborBombCount);
}