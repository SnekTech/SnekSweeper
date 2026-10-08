using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GameHistory;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.CursorManagement;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.Session;

public abstract record GridSession
{
    public sealed record AwaitingFirstReveal(Grid Grid, MineLayout Layout, GridIndex? RequiredFirstIndex) : GridSession;

    public sealed record Running(Grid Grid, ImmutableStack<GridOutcome.BatchRevealed> UndoStack, RunStartInfo StartInfo)
        : GridSession;

    public sealed record Finished : GridSession;

    public abstract record Event
    {
        public sealed record PlayerInput(GridInput Input) : Event;
        public sealed record UndoRequested : Event;
    }

    public abstract record Effect
    {
        public sealed record PaintBoard : Effect;
        public sealed record Render(GridOutcome Outcome) : Effect;
        public sealed record TriggerInitEffects : Effect;
        public sealed record OngoingRunChanged(OngoingGame OngoingGame) : Effect;
        public sealed record IncreaseCombo : Effect;
        public sealed record FinishRun(GameRunRecord LatestRun) : Effect;
        public sealed record AskForWinChoice : Effect;
        public sealed record AskForLoseChoice(GameRunRecord LatestRun) : Effect;
        public sealed record PlayCongratulationEffects : Effect;
    }
}

public readonly record struct SessionWithEffects(GridSession Session, IReadOnlyList<GridSession.Effect> Effects);

public static class GridSessionMachine
{
    public static SessionWithEffects Start(Grid grid, LevelSetup levelSetup) =>
        levelSetup switch
        {
            LevelSetup.Resume { OngoingGame: var ongoingGame } =>
                new SessionWithEffects(Restore(grid, ongoingGame),
                    [new GridSession.Effect.PaintBoard(), new GridSession.Effect.TriggerInitEffects()]),
            LevelSetup.NewGame { Layout: var layout, RequiredStartIndex: var requiredIndex } =>
                new SessionWithEffects(new GridSession.AwaitingFirstReveal(grid, layout, requiredIndex),
                    [new GridSession.Effect.PaintBoard()]),
            _ => throw new SwitchExpressionException(),
        };

    static GridSession Restore(Grid grid, OngoingGame ongoingGame)
    {
        var (snapshot, startInfo) = ongoingGame;
        grid.RestoreCellStates(snapshot);
        return new GridSession.Running(grid, ImmutableStack<GridOutcome.BatchRevealed>.Empty, startInfo);
    }

    public static SessionWithEffects Update(GridSession session, GridSession.Event evt,
        TimeProvider clock) =>
        (session, evt) switch
        {
            (GridSession.AwaitingFirstReveal first, GridSession.Event.PlayerInput input)
                => HandleFirstReveal(first, input.Input, clock),
            (GridSession.Running running, GridSession.Event.PlayerInput input)
                => HandleRunningInput(running, input.Input, clock),
            (GridSession.Running running, GridSession.Event.UndoRequested)
                => HandleUndo(running),

            // 该阶段不接受这个事件，有意忽略，不需要 throw
            _ => new SessionWithEffects(session, []),
        };

    static SessionWithEffects HandleFirstReveal(GridSession.AwaitingFirstReveal awaitingFirst,
        GridInput input, TimeProvider clock)
    {
        if (!AcceptsFirstInput(awaitingFirst.RequiredFirstIndex, input))
            return new SessionWithEffects(awaitingFirst, []);

        var grid = awaitingFirst.Grid;
        grid.InitCells(awaitingFirst.Layout.Lay(input.Index));
        var outcome = grid.HandleInput(input);

        var startInfo = new RunStartInfo(clock.GetLocalNow().DateTime, input.Index);

        List<GridSession.Effect> effects =
        [
            new GridSession.Effect.PaintBoard(),
            new GridSession.Effect.Render(outcome),
            new GridSession.Effect.OngoingRunChanged(new OngoingGame(grid.GetSnapshot(), startInfo)),
        ];

        return Referee.Judge(grid, outcome) switch
        {
            Surviving => new SessionWithEffects(
                new GridSession.Running(grid, ImmutableStack<GridOutcome.BatchRevealed>.Empty, startInfo),
                [.. effects, new GridSession.Effect.TriggerInitEffects()]),
            GameWin win => EndGameWin(effects, startInfo, clock, win.Bombs),
            GameLose lose => EndGameLose(grid, lose, effects, startInfo, clock),
            _ => throw new SwitchExpressionException(),
        };

        static bool AcceptsFirstInput(GridIndex? requiredFirstIndex, GridInput input) =>
            requiredFirstIndex is { } required ? input.Index == required : input is RevealAt;
    }

    static SessionWithEffects HandleRunningInput(GridSession.Running running, GridInput input, TimeProvider clock)
    {
        var grid = running.Grid;
        var outcome = grid.HandleInput(input);
        if (outcome is GridOutcome.NothingHappens) return new SessionWithEffects(running, []);

        List<GridSession.Effect> effects =
        [
            new GridSession.Effect.Render(outcome),
            new GridSession.Effect.OngoingRunChanged(new OngoingGame(grid.GetSnapshot(), running.StartInfo)),
        ];

        if (outcome is GridOutcome.BatchRevealed)
        {
            effects.Add(new GridSession.Effect.IncreaseCombo());
        }

        return Referee.Judge(grid, outcome) switch
        {
            Surviving => new SessionWithEffects(running with { UndoStack = PushIfRevealed(running.UndoStack, outcome) },
                effects),
            GameWin win => EndGameWin(effects, running.StartInfo, clock, win.Bombs),
            GameLose lose => EndGameLose(grid, lose, effects, running.StartInfo, clock),
            _ => throw new SwitchExpressionException(),
        };

        ImmutableStack<GridOutcome.BatchRevealed> PushIfRevealed(ImmutableStack<GridOutcome.BatchRevealed> undoStack,
            GridOutcome gridOutcome) => gridOutcome is GridOutcome.BatchRevealed batchRevealed
            ? undoStack.Push(batchRevealed)
            : undoStack;
    }

    static SessionWithEffects HandleUndo(GridSession.Running running)
    {
        if (running.UndoStack.IsEmpty) return new SessionWithEffects(running, []);

        var stack = running.UndoStack.Pop(out var lastReveal);
        var grid = running.Grid;
        var outcome = grid.CoverCells(lastReveal.Cells.Select(cell => cell.Info.Index));

        return new SessionWithEffects(running with { UndoStack = stack },
        [
            new GridSession.Effect.Render(outcome),
            new GridSession.Effect.OngoingRunChanged(new OngoingGame(grid.GetSnapshot(), running.StartInfo)),
        ]);
    }

    static SessionWithEffects EndGameWin(IReadOnlyList<GridSession.Effect> effects,
        RunStartInfo startInfo, TimeProvider clock, bool[,] bombs)
    {
        var latestRun = GameRunRecord.FromRun(startInfo, clock.GetLocalNow().DateTime, true, bombs);

        IReadOnlyList<GridSession.Effect> winEffects =
        [
            new GridSession.Effect.FinishRun(latestRun),
            new GridSession.Effect.PlayCongratulationEffects(),
            new GridSession.Effect.AskForWinChoice(),
        ];

        return new SessionWithEffects(new GridSession.Finished(), [.. effects, .. winEffects]);
    }

    static SessionWithEffects EndGameLose(Grid grid, GameLose gameLose, IReadOnlyList<GridSession.Effect> effects,
        RunStartInfo startInfo, TimeProvider clock)
    {
        var latestRun = GameRunRecord.FromRun(startInfo, clock.GetLocalNow().DateTime, false, gameLose.Bombs);

        IReadOnlyList<GridSession.Effect> loseEffects =
        [
            new GridSession.Effect.FinishRun(latestRun),
            new GridSession.Effect.Render(MarkErrors(grid, gameLose.CellsInThisBatch)),
            new GridSession.Effect.AskForLoseChoice(latestRun),
        ];

        return new SessionWithEffects(new GridSession.Finished(), [.. effects, .. loseEffects]);
    }

    static GridOutcome MarkErrors(Grid grid, IReadOnlyList<GridIndex> cells) =>
        new GridOutcome.ErrorsMarked([
            .. cells.Select(index => grid.ApplyCommand(index, new CellCommand.MarkError())).OfType<CellOutcome>(),
        ]);
}

public static class GridSessionCursorExtensions
{
    extension(GridSession session)
    {
        public CursorPolicy CursorPolicy => session switch
        {
            GridSession.AwaitingFirstReveal { RequiredFirstIndex: { } lockedIndex } =>
                new CursorPolicy.LockedTo(lockedIndex),
            _ => new CursorPolicy.FollowPointer(),
        };
    }
}