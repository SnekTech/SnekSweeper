using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.StateMachine.States;

public class RevealedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override async Task OnEnterAsync()
    {
        await Cell.Cover.RevealAsync();
    }

    public override async Task OnExitAsync()
    {
        await Cell.Cover.PutOnAsync();
    }

    public override Task RevealAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SwitchFlagAsync()
    {
        return Task.CompletedTask;
    }

    public override Task PutOnCoverAsync()
    {
        return ChangeStateAsync<CoveredState>();
    }
}