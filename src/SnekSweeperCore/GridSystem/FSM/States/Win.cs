namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class Win(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken ct = default)
    {
        // todo: show congratulation effects before leaving

        StateMachine.Context.OnWin();
        return Task.CompletedTask;
    }
}