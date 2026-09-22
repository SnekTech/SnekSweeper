using AwesomeAssertions;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.CursorManagement;

namespace BasicTests.GridTests;

public class CursorStateTests
{
    static readonly GridIndex Cell1 = new(1, 1);
    static readonly GridIndex Cell2 = new(2, 2);

    static readonly Pointer OnCell1 = new Pointer.OnGrid(Cell1);
    static readonly Pointer OnCell2 = new Pointer.OnGrid(Cell2);
    static readonly Pointer Outside = new Pointer.OffGrid();
    
    [Test]
    public void cursor_start_as_hidden()
    {
        var initial = CursorState.Initial;

        initial.Should().BeOfType<CursorState.Hidden>();
    }

    [Test]
    public void show_hidden_cursor_at_pointer_shows_it()
    {
        var hidden = new CursorState.Hidden();

        var nextState = hidden.ShowAt(OnCell1);

        nextState.Should().Be(new CursorState.Free(Cell1));
    }

    [Test]
    public void show_locked_cursor_at_pointer_wont_move_it()
    {
        var locked = new CursorState.Locked(Cell1);

        var onGridNext = locked.ShowAt(OnCell2);
        var offGridNext = locked.ShowAt(Outside);
        
        onGridNext.Should().Be(locked);
        offGridNext.Should().Be(locked);
    }

    [Test]
    public void show_free_cursor_at_off_grid_pointer_hides_it()
    {
        var free = new CursorState.Free(Cell1);
        
        var next = free.ShowAt(Outside);

        next.Should().Be(new CursorState.Hidden());
    }

    [Test]
    public void show_free_cursor_at_different_cell_moves_it()
    {
        var free = new CursorState.Free(Cell1);
        
        var next = free.ShowAt(OnCell2);

        next.Should().Be(new CursorState.Free(Cell2));
    }

    [Test]
    public void unlock_a_locked_cursor_remains_in_place()
    {
        var locked = new CursorState.Locked(Cell1);

        var next = locked.Unlock();

        next.Should().Be(new CursorState.Free(Cell1));
    }

    [Test]
    public void can_lock_all_states()
    {
        var hidden = new CursorState.Hidden();
        var free = new CursorState.Free(Cell1);
        var locked = new CursorState.Locked(Cell1);
        var lockedAtCell2 = new CursorState.Locked(Cell2);

        var next1 = hidden.LockTo(Cell2);
        var next2 = free.LockTo(Cell2);
        var next3 = locked.LockTo(Cell2);

        next1.Should().Be(lockedAtCell2);
        next2.Should().Be(lockedAtCell2);
        next3.Should().Be(lockedAtCell2);
    }

    [Test]
    public void unlock_a_non_locked_cursor_do_nothing()
    {
        var hidden = new CursorState.Hidden();
        var free = new CursorState.Free(Cell1);

        var hiddenNext = hidden.Unlock();
        var freeNext = free.Unlock();

        hiddenNext.Should().Be(hidden);
        freeNext.Should().Be(free);
    }

    [Test]
    public void show_a_hidden_cursor_off_grid_should_remain_hidden()
    {
        var hidden = new CursorState.Hidden();

        var next = hidden.ShowAt(Outside);

        next.Should().Be(new CursorState.Hidden());
    }
}