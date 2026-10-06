using AwesomeAssertions;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.CursorManagement;

namespace BasicTests.GridTests;

public class CursorStateSpecs
{
    static readonly GridIndex Cell1 = new(1, 1);
    static readonly GridIndex Cell2 = new(2, 2);

    static readonly PointerTarget OnCell1 = new PointerTarget.OnGrid(Cell1);
    static readonly PointerTarget OnCell2 = new PointerTarget.OnGrid(Cell2);
    static readonly PointerTarget Outside = new PointerTarget.OffGrid();

    [Test]
    public void follow_pointer_shows_at_the_pointer_cell()
    {
        var state = CursorState.From(new CursorPolicy.FollowPointer(), OnCell1);

        state.Should().Be(new CursorState.Free(Cell1));
    }

    [Test]
    public void follow_pointer_hides_when_pointer_is_off_grid()
    {
        var state = CursorState.From(new CursorPolicy.FollowPointer(), Outside);

        state.Should().Be(new CursorState.Hidden());
    }

    [Test]
    public void locked_policy_ignores_the_pointer()
    {
        var stateWhenChangeIndex = CursorState.From(new CursorPolicy.LockedTo(Cell1), OnCell2);
        var stateWhenOutside = CursorState.From(new CursorPolicy.LockedTo(Cell1), Outside);

        stateWhenChangeIndex.Should().Be(new CursorState.Locked(Cell1));
        stateWhenOutside.Should().Be(new CursorState.Locked(Cell1));
    }
}