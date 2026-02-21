namespace SnekSweeperCore.CellSystem.StateMachine.States;

public class BombRevealedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken cancellationToken = default)
    {
        HumbleCell.MarkAsBombRevealed();
        return Task.CompletedTask;
    }
}