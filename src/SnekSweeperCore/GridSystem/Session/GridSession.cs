using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.CursorManagement;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.Session;

public abstract record GridSession
{
    public sealed record AwaitingFirstReveal(Grid Grid, MineLayout Layout, GridIndex? RequiredFirstIndex) : GridSession;
    public sealed record Running(Grid Grid, ImmutableStack<GridOutcome.BatchRevealed> UndoStack) : GridSession;
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
        public sealed record StartOngoingRun(GridSnapshot Snapshot, RunStartInfo StartInfo) : Effect;
        public sealed record UpdateOngoingRun(GridSnapshot Snapshot) : Effect;
        public sealed record IncreaseCombo : Effect;
        public sealed record FinishRun(bool Winning, bool[,] Bombs) : Effect;
        public sealed record AskForWinChoice : Effect;
        public sealed record AskForLoseChoice : Effect;
        public sealed record PlayCongratulationEffects : Effect;
    }
}

public readonly record struct SessionWithEffects(GridSession Session, IReadOnlyList<GridSession.Effect> Effects);

public static class GridSessionMachine
{
    public static SessionWithEffects Start(Grid grid, LevelSetup levelSetup) =>
        levelSetup switch
        {
            LevelSetup.Resume { Snapshot: var snapshot } =>
                new SessionWithEffects(Restore(grid, snapshot),
                    [new GridSession.Effect.PaintBoard(), new GridSession.Effect.TriggerInitEffects()]),
            LevelSetup.NewGame { Layout: var layout, RequiredStartIndex: var requiredIndex } =>
                new SessionWithEffects(new GridSession.AwaitingFirstReveal(grid, layout, requiredIndex),
                    [new GridSession.Effect.PaintBoard()]),
            _ => throw new SwitchExpressionException(),
        };

    static GridSession Restore(Grid grid, GridSnapshot snapshot)
    {
        grid.RestoreCellStates(snapshot);
        return new GridSession.Running(grid, ImmutableStack<GridOutcome.BatchRevealed>.Empty);
    }

    public static SessionWithEffects Update(GridSession session, GridSession.Event evt,
        TimeProvider clock) =>
        (session, evt) switch
        {
            (GridSession.AwaitingFirstReveal first, GridSession.Event.PlayerInput input)
                => HandleFirstReveal(first, input.Input, clock),
            (GridSession.Running running, GridSession.Event.PlayerInput input)
                => HandleRunningInput(running, input.Input),
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

        List<GridSession.Effect> effects =
        [
            new GridSession.Effect.Render(outcome),
            new GridSession.Effect.StartOngoingRun(grid.GetSnapshot(),
                new RunStartInfo(clock.GetLocalNow().DateTime, input.Index)),
        ];

        return Referee.Judge(grid, outcome) switch
        {
            Surviving => new SessionWithEffects(
                new GridSession.Running(grid, ImmutableStack<GridOutcome.BatchRevealed>.Empty),
                [.. effects, new GridSession.Effect.TriggerInitEffects()]),
            GameWin win => EndGameWin(win, effects),
            GameLose lose => EndGameLose(grid, lose, effects),
            _ => throw new SwitchExpressionException(),
        };

        static bool AcceptsFirstInput(GridIndex? requiredFirstIndex, GridInput input) =>
            requiredFirstIndex is { } required ? input.Index == required : input is RevealAt;
    }

    static SessionWithEffects HandleRunningInput(GridSession.Running running, GridInput input)
    {
        var grid = running.Grid;
        var outcome = grid.HandleInput(input);
        if (outcome is GridOutcome.NothingHappens) return new SessionWithEffects(running, []);

        List<GridSession.Effect> effects =
        [
            new GridSession.Effect.Render(outcome),
            new GridSession.Effect.UpdateOngoingRun(grid.GetSnapshot()),
        ];

        if (outcome is GridOutcome.BatchRevealed)
        {
            effects.Add(new GridSession.Effect.IncreaseCombo());
        }

        return Referee.Judge(grid, outcome) switch
        {
            Surviving => new SessionWithEffects(running with { UndoStack = PushIfRevealed(running.UndoStack, outcome) },
                effects),
            GameWin win => EndGameWin(win, effects),
            GameLose lose => EndGameLose(grid, lose, effects),
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
            new GridSession.Effect.UpdateOngoingRun(grid.GetSnapshot()),
        ]);
    }

    static IReadOnlyList<GridSession.Effect> GetWinEffects(GameWin gameWin) =>
    [
        new GridSession.Effect.FinishRun(true, gameWin.Bombs),
        new GridSession.Effect.PlayCongratulationEffects(),
        new GridSession.Effect.AskForWinChoice(),
    ];

    static IReadOnlyList<GridSession.Effect> GetLoseEffects(Grid grid, GameLose gameLose) =>
    [
        new GridSession.Effect.FinishRun(false, gameLose.Bombs),
        new GridSession.Effect.Render(MarkErrors(grid, gameLose.CellsInThisBatch)),
        new GridSession.Effect.AskForLoseChoice(),
    ];

    static SessionWithEffects EndGameWin(GameWin gameWin, IReadOnlyList<GridSession.Effect> effects) =>
        new(new GridSession.Finished(), [.. effects, .. GetWinEffects(gameWin)]);

    static SessionWithEffects EndGameLose(Grid grid, GameLose gameLose, IReadOnlyList<GridSession.Effect> effects) =>
        new(new GridSession.Finished(), [.. effects, .. GetLoseEffects(grid, gameLose)]);

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