using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.StateMachine.States;

public class CoveredState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override Task OnEnterAsync()
    {
        return Task.CompletedTask;
    }

    public override Task OnExitAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task RevealAsync()
    {
        await ChangeStateAsync<RevealedState>();
    }

    public override Task SwitchFlagAsync()
    {
        return ChangeStateAsync<FlaggedState>();
    }

    public override Task PutOnCoverAsync()
    {
        return Task.CompletedTask;
    }
}