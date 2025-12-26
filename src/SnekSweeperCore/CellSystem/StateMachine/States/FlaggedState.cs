namespace SnekSweeperCore.CellSystem.StateMachine.States;

public class FlaggedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken cancellationToken = default)
    {
        await HumbleCell.Flag.RaiseAsync(cancellationToken);
    }

    public override async Task OnExitAsync(CancellationToken cancellationToken = default)
    {
        await HumbleCell.Flag.PutDownAsync(cancellationToken);
    }
}