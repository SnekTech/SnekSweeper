using System.Runtime.CompilerServices;
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

    public void ShowAt(Pointer pointer)
    {
        if (_isLocked) return;

        switch (pointer)
        {
            case Pointer.OnGrid onGrid:
                Show();
                Position = onGrid.Index.ToPosition();
                break;
            case Pointer.OffGrid:
                Hide();
                break;
            default:
                throw new SwitchExpressionException();
        }
    }

    public void LockTo(GridIndex gridIndex)
    {
        // todo: decide whether to check if index is within grid
        ShowAt(new Pointer.OnGrid(gridIndex));

        SelfModulate = Colors.Blue;
        _isLocked = true;
    }

    public void Unlock()
    {
        SelfModulate = Colors.White;
        _isLocked = false;
    }
}
