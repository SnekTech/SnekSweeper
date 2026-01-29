namespace SnekSweeperCore.GridSystem.FSM.States;

public class Lose(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken ct = default)
    {
        StateMachine.Context.OnLose();
        return Task.CompletedTask;
    }
}