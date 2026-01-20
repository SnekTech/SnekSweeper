namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class RegularInstantiated(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput is PrimaryReleased)
        {
            await StateMachine.ChangeStateAsync<GameStart>(ct);
            // grid.HandleInput?
        }
    }
}