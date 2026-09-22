using System.Runtime.CompilerServices;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.CursorManagement;

namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D, IGridCursor
{
    const int CursorZIndex = 1;
    static readonly Color OriginalColor = Colors.White;
    static readonly Color LockedColor = Colors.Blue;
    
    CursorState _state = CursorState.Initial;

    public override void _Ready()
    {
        ZIndex = CursorZIndex;
        Hide();
    }

    public void ShowAt(Pointer pointer)
    {
        ApplyState(_state.ShowAt(pointer));
    }

    public void LockTo(GridIndex gridIndex)
    {
        ApplyState(_state.LockTo(gridIndex));
    }

    public void Unlock()
    {
        ApplyState(_state.Unlock());
    }

    void ApplyState(CursorState next)
    {
        if (next == _state) return;

        _state = next;
        
        switch (_state)
        {
            case CursorState.Free free:
                Show();
                Position = free.Index.ToPosition();
                SelfModulate = OriginalColor;
                break;
            case CursorState.Locked locked:
                Show();
                Position = locked.Index.ToPosition();
                SelfModulate = LockedColor;
                break;
            case CursorState.Hidden:
                Hide();
                break;
            default:
                throw new SwitchExpressionException();
        }
    }
}
