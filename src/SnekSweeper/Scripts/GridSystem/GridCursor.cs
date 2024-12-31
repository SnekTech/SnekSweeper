using Godot;
using SnekSweeper.Constants;

namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D
{
    private const int CursorZIndex = 1;
    
    public override void _Ready()
    {
        ZIndex = CursorZIndex;
        Hide();
    }

    public void ShowAtHoveringCell((int i, int j) hoveringGridIndex)
    {
        Show();
        var (i, j) = hoveringGridIndex;
        const int cellSize = CoreStats.CellSizePixels;
        Position = new Vector2(j * cellSize, i * cellSize);
    }
}