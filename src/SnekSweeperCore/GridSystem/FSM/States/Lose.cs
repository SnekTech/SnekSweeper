namespace SnekSweeperCore.GridSystem.FSM.States;

public class Lose(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken ct = default)
    {
        // todo: mark cells in question
        
        StateMachine.Context.OnLose();
        return Task.CompletedTask;
    }
}