namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D
{
    private const int CursorZIndex = 1;
    
    public override void _Ready()
    {
        ZIndex = CursorZIndex;
        Hide();
    }

    public void ShowAtHoveringCell(GridIndex hoveringGridIndex)
    {
        Show();
        Position = hoveringGridIndex.ToPosition();
    }
}