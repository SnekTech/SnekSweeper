using System.Runtime.CompilerServices;

namespace SnekSweeperCore.GridSystem.CursorManagement;

public abstract record CursorState
{
    public sealed record Free(GridIndex Index) : CursorState;
    public sealed record Locked(GridIndex Index) : CursorState;
    public sealed record Hidden : CursorState;
}

public abstract record CursorPolicy
{
    public sealed record FollowPointer : CursorPolicy;
    public sealed record LockedTo(GridIndex Index) : CursorPolicy;
}

public static class CursorStateExtensions
{
    extension(CursorState)
    {
        public static CursorState From(CursorPolicy policy, PointerTarget target) => policy switch
        {
            CursorPolicy.LockedTo lockedTo => new CursorState.Locked(lockedTo.Index),
            CursorPolicy.FollowPointer => target switch
            {
                PointerTarget.OnGrid onGrid => new CursorState.Free(onGrid.Index),
                PointerTarget.OffGrid => new CursorState.Hidden(),
                _ => throw new SwitchExpressionException(),
            },
            _ => throw new SwitchExpressionException(),
        };
    }
}
