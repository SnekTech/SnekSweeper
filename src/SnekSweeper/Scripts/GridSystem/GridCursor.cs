using Godot;
using SnekSweeper.Constants;

namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D
{
    public void ShowAtHoveringCell((int i, int j) hoveringGridIndex)
    {
        Show();
        var (i, j) = hoveringGridIndex;
        Position = new Vector2(j * CoreStats.CellSizePixels, i * CoreStats.CellSizePixels);
    }
}