using System.Threading.Tasks;

namespace SnekSweeper.CellSystem.StateMachine.States;

public class FlaggedState(CellStateMachine stateMachine) : CellState(stateMachine)
{
    public override async Task OnEnterAsync()
    {
        await Cell.Flag.RaiseAsync();
    }

    public override async Task OnExitAsync()
    {
        await Cell.Flag.PutDownAsync();
    }
}