using SnekSweeperCore.FSM;
using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem.FSM;

public abstract class GridState(GridStateMachine stateMachine) : IState
{
    protected GridStateMachine StateMachine => stateMachine;

    public virtual Task OnEnterAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task OnExitAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) => Task.CompletedTask;
    public virtual Task JudgedBy(Referee referee, CancellationToken ct = default) => Task.CompletedTask;
}