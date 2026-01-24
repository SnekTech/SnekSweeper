using SnekSweeperCore.FSM;
using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem.FSM;

public abstract class GridState(GridStateMachine stateMachine) : IState
{
    protected Grid Grid => stateMachine.Context.Grid;
    protected IHumbleGrid HumbleGrid => stateMachine.Context.HumbleGrid;

    protected Task ChangeStateAsync<T>(CancellationToken ct = default) where T : GridState =>
        stateMachine.ChangeStateAsync<T>(ct);

    public virtual Task OnEnterAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task OnExitAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) => Task.CompletedTask;
    public virtual Task JudgedBy(Referee referee, CancellationToken ct = default) => Task.CompletedTask;
}

public abstract class Instantiated(GridStateMachine stateMachine)
    : GridState(stateMachine);