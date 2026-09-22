using System.Runtime.CompilerServices;

namespace SnekSweeperCore.GridSystem.CursorManagement;

public abstract record CursorState
{
    public sealed record Free(GridIndex Index) : CursorState;
    public sealed record Locked(GridIndex Index) : CursorState;
    public sealed record Hidden : CursorState;
}

public static class CursorStateExtensions
{
    extension(CursorState cursorState)
    {
        public static CursorState Initial => new CursorState.Hidden();

        public CursorState LockTo(GridIndex index) => new CursorState.Locked(index);

        public CursorState Unlock() => cursorState switch
        {
            // only locked can be unlocked
            CursorState.Locked locked => new CursorState.Free(locked.Index),
            _ => cursorState,
        };

        public CursorState ShowAt(PointerTarget target) =>
            (cursorState, target) switch
            {
                (CursorState.Locked locked, _) => locked, // locked cursor does not move
                (_, PointerTarget.OffGrid) => new CursorState.Hidden(), // other states hide when pointer off grid
                (_, PointerTarget.OnGrid onGrid) => new CursorState.Free(onGrid
                    .Index), // other states show when pointer on grid
                _ => throw new SwitchExpressionException(),
            };
    }
}
