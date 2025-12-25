namespace SnekSweeperCore.CellSystem.StateMachine.States;

public class FlaggedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override async Task OnEnterAsync()
    {
        await HumbleCell.Flag.RaiseAsync();
    }

    public override async Task OnExitAsync()
    {
        await HumbleCell.Flag.PutDownAsync();
    }
}