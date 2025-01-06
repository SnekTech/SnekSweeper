using System.Threading.Tasks;
using SnekSweeper.CellSystem.StateMachine.States;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public class CellStateMachine(Cell cell) : StateMachine<CellState>
{
    public readonly Cell Cell = cell;

    protected override void PopulateStateInstances()
    {
        StateInstances[typeof(CoveredState)] = new CoveredState(this);
        StateInstances[typeof(RevealedState)] = new RevealedState(this);
        StateInstances[typeof(FlaggedState)] = new FlaggedState(this);
    }

    public async Task RevealAsync()
    {
        if (CurrentState == null)
            return;

        await CurrentState.RevealAsync();
    }

    public async Task PutOnCoverAsync()
    {
        if (CurrentState == null)
            return;
        await CurrentState.PutOnCoverAsync();
    }

    public async Task SwitchFlagAsync()
    {
        if (CurrentState == null)
            return;

        await CurrentState.SwitchFlagAsync();
    }
}