using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D, IGridCursor
{
    const int CursorZIndex = 1;
    bool _isLocked;

    public override void _Ready()
    {
        ZIndex = CursorZIndex;
        Hide();
    }

    public void ShowAt(GridIndex gridIndex, GridSize gridSize)
    {
        if (_isLocked) return;

        if (!gridIndex.IsWithin(gridSize))
        {
            Hide();
            return;
        }

        Show();
        Position = gridIndex.ToPosition();
    }

    public void LockTo(GridIndex gridIndex, GridSize gridSize)
    {
        ShowAt(gridIndex, gridSize);

        SelfModulate = Colors.Blue;
        _isLocked = true;
    }

    public void Unlock()
    {
        SelfModulate = Colors.White;
        _isLocked = false;
    }
}