using System;
using SnekSweeper.CellSystem.Components;
using SnekSweeper.SkinSystem;

namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    event Action PrimaryReleased;
    event Action PrimaryDoubleClicked;
    event Action SecondaryReleased;

    ICover Cover { get; }
    IFlag Flag { get; }
    void SetContent(bool hasBomb, int neighborBombCount);
    void SetPosition((int i, int j) gridIndex);
    void UseSkin(ISkin newSkin);
}