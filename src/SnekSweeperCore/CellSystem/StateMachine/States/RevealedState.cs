namespace SnekSweeperCore.CellSystem.StateMachine.States;

public class RevealedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken cancellationToken = default)
    {
        await HumbleCell.Cover.RevealAsync(cancellationToken);
    }

    public override async Task OnExitAsync(CancellationToken cancellationToken = default)
    {
        await HumbleCell.Cover.PutOnAsync(cancellationToken);
    }
}