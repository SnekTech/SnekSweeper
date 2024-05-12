using SnekSweeper.GridSystem;

namespace SnekSweeper.CellSystem;

public class Cell
{
    
    private IHumbleCell _humbleCell;
    private Grid _parent;

    public Cell(IHumbleCell humbleCell, Grid parent, (int i, int j) gridIndex, bool hasBomb)
    {
        _humbleCell = humbleCell;
        _parent = parent;
        GridIndex = gridIndex;
        HasBomb = hasBomb;
    }

    public void Init()
    {
        _humbleCell.Init(this);
    }
    
    public (int i, int j) GridIndex { get; }
    public bool HasBomb { get; }
    
    // todo: calculate neighbor
    public int NeighborBombCount => HasBomb ? -1 : 0; 
}