using AwesomeAssertions;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.CursorManagement;
using SnekSweeperCore.GridSystem.Session;
using SnekSweeperCore.LevelManagement;

namespace BasicTests.GridTests;

public class GridSessionSpecs
{
    // 3x3，雷只在 (0,0)
    static readonly bool[,] Bombs = new[,]
    {
        { true, false, false },
        { false, false, false },
        { false, false, false },
    };

    static readonly GridIndex Cell1 = new(1, 1);
    static readonly MineLayout.Fixed DefaultLayout = new(Bombs);

    [Test]
    public void first_click_does_not_push_undo()
    {
        var layout = DefaultLayout;
        var grid = new Grid(layout.Size);
        var firstIndex = new GridIndex(0, 1);
        var secondIndex = new GridIndex(1, 0);
        var firstReveal = new GridSession.Event.PlayerInput(new RevealAt(firstIndex));
        var secondReveal = new GridSession.Event.PlayerInput(new RevealAt(secondIndex));

        var (initialSession, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));

        var (next, effects) = GridSessionMachine.Update(initialSession, firstReveal, TimeProvider.System);

        next.Should().BeOfType<GridSession.Running>().Which.UndoStack.Should().BeEmpty();
        grid.StateAt(firstIndex).IsRevealed.Should().BeTrue();

        var (next2, _) = GridSessionMachine.Update(next, secondReveal, TimeProvider.System);
        next2.Should().BeOfType<GridSession.Running>().Which.UndoStack.Should().NotBeEmpty();
    }

    [Test]
    public void cursor_is_locked_to_recorded_start_index_while_awaiting_first_reveal()
    {
        var layout = DefaultLayout;
        var grid = new Grid(layout.Size);

        var (session, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, Cell1));

        session.CursorPolicy.Should().Be(new CursorPolicy.LockedTo(Cell1));
    }

    [Test]
    public void cursor_follows_pointer_in_other_phases()
    {
        var layout = DefaultLayout;
        var grid = new Grid(layout.Size);

        var (initSession, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));

        initSession.CursorPolicy.Should().Be(new CursorPolicy.FollowPointer());

        // 点 (0,1)：邻居雷数 1，不会扩散，落在 Running 而不是 Win
        var (running, _) = GridSessionMachine.Update(initSession,
            new GridSession.Event.PlayerInput(new RevealAt(new GridIndex(0, 1))), TimeProvider.System);
        running.CursorPolicy.Should().Be(new CursorPolicy.FollowPointer());
    }
}