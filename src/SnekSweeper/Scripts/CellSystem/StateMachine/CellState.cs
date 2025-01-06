using System.Threading.Tasks;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public abstract class CellState(CellStateMachine stateMachine) : IState
{
    protected Cell Cell => stateMachine.Cell;

    public abstract Task OnEnterAsync();

    public abstract Task OnExitAsync();

    public abstract Task RevealAsync();

    public abstract Task PutOnCoverAsync();

    public abstract Task SwitchFlagAsync();

    protected Task ChangeStateAsync<T>() where T : CellState
    {
        return stateMachine.ChangeStateAsync<T>();
    }
}