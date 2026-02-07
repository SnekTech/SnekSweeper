namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class Win(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken ct = default)
    {
        HumbleGrid.PlayCongratulationEffects();

        // todo: task-based popup?
        return Task.CompletedTask;
    }
}