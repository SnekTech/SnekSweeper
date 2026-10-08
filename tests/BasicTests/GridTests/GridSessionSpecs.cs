using AwesomeAssertions;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GameHistory;
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

    // 2x2，雷在 (0,0) 与 (1,1)：两个安全格都紧邻两颗雷，点一格只翻开一格
    static readonly bool[,] TwoBombs = new[,]
    {
        { true, false },
        { false, true },
    };

    // 1x2，雷在 (0,0)：唯一的安全格点开即通关
    static readonly bool[,] SingleSafeCell = new[,] { { true, false } };

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

    [Test]
    public void winning_in_running_records_a_winning_run()
    {
        // 2x2，雷在 (0,0) 与 (1,1)：两个安全格各自与两颗雷相邻（计数 2），
        // 所以第一次点击只翻开一格 ⇒ 进入 Running；第二次点击才补完棋盘 ⇒ 赢。
        var layout = new MineLayout.Fixed(TwoBombs);
        var grid = new Grid(layout.Size);
        var firstIndex = new GridIndex(0, 1);
        var lastSafeIndex = new GridIndex(1, 0);
        var firstClock = new FixedTimeProvider(new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero));
        var endClock = new FixedTimeProvider(new DateTimeOffset(2026, 1, 1, 10, 5, 0, TimeSpan.Zero));

        var (awaiting, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));
        var (running, _) = GridSessionMachine.Update(awaiting, Click(firstIndex), firstClock);
        running.Should().BeOfType<GridSession.Running>();

        var (end, effects) = GridSessionMachine.Update(running, Click(lastSafeIndex), endClock);

        end.Should().Be(new GridSession.Finished());

        var record = FinishRunRecord(effects);
        record.Winning.Should().BeTrue();
        record.StartIndex.Should().Be(firstIndex);
        // 起点时间来自"首击那一刻"，终点时间来自"结算那一刻"。
        // 两个时钟刻意不同：若实现误用"当前时间"当起点，这里会失败。
        record.Duration.StartAt.Should().Be(firstClock.GetLocalNow().DateTime);
        record.Duration.EndAt.Should().Be(endClock.GetLocalNow().DateTime);
        record.BombMatrix.Should().BeEquivalentTo(TwoBombs);
    }

    [Test]
    public void losing_in_running_records_a_losing_run()
    {
        var layout = DefaultLayout;
        var grid = new Grid(layout.Size);
        var firstIndex = new GridIndex(0, 1);
        var bombIndex = new GridIndex(0, 0);

        var (awaiting, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));
        var (running, _) = GridSessionMachine.Update(awaiting, Click(firstIndex), TimeProvider.System);
        running.Should().BeOfType<GridSession.Running>();

        var (end, effects) = GridSessionMachine.Update(running, Click(bombIndex), TimeProvider.System);

        end.Should().Be(new GridSession.Finished());
        var record = FinishRunRecord(effects);
        record.Winning.Should().BeFalse();
        record.StartIndex.Should().Be(firstIndex);
    }

    [Test]
    public void first_click_win_is_recorded_as_winning()
    {
        // 1x2，雷在 (0,0)：点开唯一的非雷格即通关，走的是 AwaitingFirstReveal 的结算分支
        var layout = new MineLayout.Fixed(SingleSafeCell);
        var grid = new Grid(layout.Size);
        var safeIndex = new GridIndex(0, 1);

        var (awaiting, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));
        var (end, effects) = GridSessionMachine.Update(awaiting, Click(safeIndex), TimeProvider.System);

        end.Should().Be(new GridSession.Finished());
        var record = FinishRunRecord(effects);
        record.Winning.Should().BeTrue();
        record.StartIndex.Should().Be(safeIndex);
    }

    [Test]
    public void lose_choice_carries_the_run_that_just_finished()
    {
        var layout = DefaultLayout;
        var grid = new Grid(layout.Size);

        var (awaiting, _) = GridSessionMachine.Start(grid, new LevelSetup.NewGame(layout, null));
        var (running, _) = GridSessionMachine.Update(awaiting, Click(new GridIndex(0, 1)), TimeProvider.System);
        var (_, effects) = GridSessionMachine.Update(running, Click(new GridIndex(0, 0)), TimeProvider.System);

        // 弹窗要用的那条记录必须就是刚落盘的那一条（Retry 依赖它）
        var finishRun = FinishRunRecord(effects);
        var askedRun = effects.OfType<GridSession.Effect.AskForLoseChoice>().Single().LatestRun;

        askedRun.Should().BeSameAs(finishRun);
    }

    [Test]
    public void resume_keeps_the_original_start_info()
    {
        var layout = new MineLayout.Fixed(TwoBombs);
        var grid = new Grid(layout.Size);
        var storedStartInfo = new RunStartInfo(new DateTime(2026, 1, 1), new GridIndex(1, 0));
        var snapshot = new GridSnapshot(
            new[,]
            {
                { CellSnapshotState.Covered, CellSnapshotState.Covered },
                { CellSnapshotState.Covered, CellSnapshotState.Covered },
            },
            TwoBombs);

        var (session, _) = GridSessionMachine.Start(grid,
            new LevelSetup.Resume(new OngoingGame(snapshot, storedStartInfo)));

        var running = session.Should().BeOfType<GridSession.Running>().Which;
        running.StartInfo.Should().Be(storedStartInfo);

        // 进行中的更新也带着最初的 startInfo —— 不会被"现在"覆盖掉
        var (next, effects) = GridSessionMachine.Update(running, Click(new GridIndex(0, 1)), TimeProvider.System);

        next.Should().BeOfType<GridSession.Running>().Which.StartInfo.Should().Be(storedStartInfo);
        effects.OfType<GridSession.Effect.OngoingRunChanged>()
            .Single().OngoingGame.StartInfo.Should().Be(storedStartInfo);
    }

    static GridSession.Event Click(GridIndex index) => new GridSession.Event.PlayerInput(new RevealAt(index));

    static GameRunRecord FinishRunRecord(IReadOnlyList<GridSession.Effect> effects) =>
        effects.OfType<GridSession.Effect.FinishRun>().Single().LatestRun;
}

/// <summary>固定时钟：让"起点时间"这类断言不依赖真实时间与机器时区。</summary>
sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
