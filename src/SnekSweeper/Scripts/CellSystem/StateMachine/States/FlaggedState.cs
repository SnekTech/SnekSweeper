using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.StateMachine.States;

public class FlaggedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override Task OnEnterAsync()
    {
        Cell.Flag.Raise();
        return Task.CompletedTask;
    }

    public override Task OnExitAsync()
    {
        return Task.CompletedTask;
    }

    public override Task RevealAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SwitchFlagAsync()
    {
        return ChangeStateAsync<CoveredState>();
    }

    public override Task PutOnCoverAsync()
    {
        return Task.CompletedTask;
    }
}