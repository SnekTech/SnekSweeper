using SnekSweeperCore.CellSystem.StateMachine.States;
using SnekSweeperCore.FSM;

namespace SnekSweeperCore.CellSystem.StateMachine;

public class CellStateMachine(IHumbleCell humbleCell) : StateMachine<CellState>
{
    public readonly IHumbleCell HumbleCell = humbleCell;

    protected override void SetupStateInstances()
    {
        var coveredState = new CoveredState(this);
        var revealedState = new RevealedState(this);
        var flaggedState = new FlaggedState(this);

        StateInstances[typeof(CoveredState)] = coveredState;
        StateInstances[typeof(RevealedState)] = revealedState;
        StateInstances[typeof(FlaggedState)] = flaggedState;

        SetupTransitions();
        return;

        void SetupTransitions()
        {
            coveredState.SetTransitions([
                new Transition(revealedState, CellRequest.RevealCover),
                new Transition(flaggedState, CellRequest.RaiseFlag),
            ]);

            revealedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutOnCover),
            ]);

            flaggedState.SetTransitions([
                new Transition(coveredState, CellRequest.PutDownFlag),
            ]);
        }
    }

    public async Task HandleCellRequestAsync(CellRequest request, CancellationToken ct = default)
    {
        if (CurrentState == null)
            return;

        foreach (var (to, onRequest, condition) in CurrentState.Transitions)
        {
            if (onRequest != request) continue;
            if (condition is not null)
            {
                if (!condition()) continue;
            }

            await ChangeStateAsync(to, ct);
            return;
        }
    }
}