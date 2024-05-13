namespace SnekSweeper.CellSystem;

public interface IHumbleCell
{
    void SetContent(Cell cell);
    void SetPosition(Cell cell);
    void PutCover();
    void Reveal();
}