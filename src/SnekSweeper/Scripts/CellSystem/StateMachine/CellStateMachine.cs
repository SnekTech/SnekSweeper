using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine(Cell cell) : StateMachine<CellState>
{
    public readonly Cell Cell = cell;

    protected override void SetupStateInstances()
    {
        StateInstances[typeof(CoveredState)] = new CoveredState(this);
        StateInstances[typeof(RevealedState)] = new RevealedState(this);
        StateInstances[typeof(FlaggedState)] = new FlaggedState(this);

        ConfigStateTransitions();
    }

    public async Task HandleCellRequestAsync(CellRequest request)
    {
        if (CurrentState == null)
            return;

        foreach (var (to, onRequest) in CurrentState.Transitions)
        {
            if (onRequest != request) continue;

            await ChangeStateAsync(to);
            return;
        }
    }

    private void ConfigStateTransitions()
    {
        var coveredState = GetState<CoveredState>();
        var revealedState = GetState<RevealedState>();
        var flaggedState = GetState<FlaggedState>();

        coveredState.AddTransition(new Transition(revealedState, CellRequest.RevealCover));
        coveredState.AddTransition(new Transition(flaggedState, CellRequest.RaiseFlag));

        revealedState.AddTransition(new Transition(coveredState, CellRequest.PutOnCover));

        flaggedState.AddTransition(new Transition(coveredState, CellRequest.PutDownFlag));
    }
}