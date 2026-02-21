namespace SnekSweeperCore.CellSystem.StateMachine.States;

public class WrongFlaggedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override Task OnEnterAsync(CancellationToken cancellationToken = default)
    {
        HumbleCell.MarkAsWrongFlagged();
        return Task.CompletedTask;
    }
}