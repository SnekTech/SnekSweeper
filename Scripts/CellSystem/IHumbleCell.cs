namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    void Init(Cell cell);
    void PutCover();
    void Reveal();
}