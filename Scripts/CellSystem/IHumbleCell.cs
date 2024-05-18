using System;
using SnekSweeper.CellSystem.Components;

namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    event Action PrimaryReleased;
    
    ICover Cover { get; }
    IFlag Flag { get; }
    void SetContent(Cell cell);
    void SetPosition((int i, int j) gridIndex);
}