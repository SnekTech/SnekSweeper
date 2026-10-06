using System.Runtime.CompilerServices;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.CursorManagement;

namespace SnekSweeper.GridSystem;

public partial class GridCursor : Sprite2D, IGridCursor
{
    const int CursorZIndex = 1;
    static readonly Color OriginalColor = Colors.White;
    static readonly Color LockedColor = Colors.Blue;

    CursorPolicy _policy = new CursorPolicy.FollowPointer();
    PointerTarget _target = new PointerTarget.OffGrid();

    public override void _Ready()
    {
        ZIndex = CursorZIndex;
        Refresh();
    }

    public void SetCursorPolicy(CursorPolicy policy)
    {
        _policy = policy;
        Refresh();
    }

    public void ShowAt(PointerTarget target)
    {
        _target = target;
        Refresh();
    }

    void Refresh() => ApplyState(CursorState.From(_policy, _target));

    void ApplyState(CursorState next)
    {
        switch (next)
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