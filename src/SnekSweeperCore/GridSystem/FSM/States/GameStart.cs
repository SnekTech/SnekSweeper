namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class GameStart(GridStateMachine stateMachine) : GridState(stateMachine)
{
    public override Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) =>
        Grid.HandleInputAsync(gridInput, ct);
}