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
                new Transition(revealedState, CellRequest.RevealCover, ct => HumbleCell.Cover.RevealAsync(ct)),
                new Transition(flaggedState, CellRequest.RaiseFlag, ct => HumbleCell.Flag.RaiseAsync(ct)),
            ]);

            revealedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutOnCover, ct => HumbleCell.Cover.PutOnAsync(ct)),
                new Transition(bombRevealedState, CellRequest.MarkError, _ =>
                {
                    HumbleCell.MarkAsBombRevealed();
                    return Task.CompletedTask;
                }, () => cell.HasBomb),
            ]);

            flaggedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutDownFlag, ct => HumbleCell.Flag.PutDownAsync(ct)),
                new Transition(wrongFlaggedState, CellRequest.MarkError, _ =>
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
                await onTransitionAsync(ct);

            await ChangeStateAsync(to, ct);
            return;
        }
    }
}