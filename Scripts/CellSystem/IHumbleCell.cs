using System;

namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    public event Action PrimaryReleased;
    void SetContent(Cell cell);
    void SetPosition((int i, int j) gridIndex);
    void PutCover();
    void Reveal();
}