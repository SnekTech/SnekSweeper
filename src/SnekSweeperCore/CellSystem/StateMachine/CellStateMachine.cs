using SnekSweeperCore.CellSystem.StateMachine.States;
using SnekSweeperCore.FSM;

namespace SnekSweeperCore.CellSystem.StateMachine;

public class CellStateMachine(Cell cell) : StateMachine<CellState>
{
    public readonly IHumbleCell HumbleCell = cell.HumbleCell;

    protected override void SetupStateInstances()
    {
        var coveredState = new CoveredState(this);
        var revealedState = new RevealedState(this);
        var flaggedState = new FlaggedState(this);
        var bombRevealedState = new BombRevealedState(this);
        var wrongFlaggedState = new WrongFlaggedState(this);

        StateInstances[typeof(CoveredState)] = coveredState;
        StateInstances[typeof(RevealedState)] = revealedState;
        StateInstances[typeof(FlaggedState)] = flaggedState;
        StateInstances[typeof(BombRevealedState)] = bombRevealedState;
        StateInstances[typeof(WrongFlaggedState)] = wrongFlaggedState;

        SetupTransitions();
        return;

        void SetupTransitions()
        {
            coveredState.SetTransitions([
                new Transition(revealedState, CellRequest.RevealCover, () => HumbleCell.Cover.RevealAsync()),
                new Transition(flaggedState, CellRequest.RaiseFlag, () => HumbleCell.Flag.RaiseAsync()),
            ]);

            revealedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutOnCover, () => HumbleCell.Cover.PutOnAsync()),
                new Transition(bombRevealedState, CellRequest.MarkError, () =>
                {
                    HumbleCell.MarkAsBombRevealed();
                    return Task.CompletedTask;
                }, () => cell.HasBomb),
            ]);

            flaggedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutDownFlag, () => HumbleCell.Flag.PutDownAsync()),
                new Transition(wrongFlaggedState, CellRequest.MarkError, () =>
                {
                    HumbleCell.MarkAsWrongFlagged();
                    return Task.CompletedTask;
                }, () => cell.IsWrongFlagged),
            ]);
        }
    }

    public async Task HandleCellRequestAsync(CellRequest request, CancellationToken ct = default)
    {
        if (CurrentState == null)
            return;

        foreach (var (to, onRequest, onTransitionAsync, condition) in CurrentState.Transitions)
        {
            if (onRequest != request) continue;
            var conditionNotMet = condition is not null && !condition();
            if (conditionNotMet) continue;

            if (onTransitionAsync != null)
                await onTransitionAsync();

            await ChangeStateAsync(to, ct);
            return;
        }
    }
}