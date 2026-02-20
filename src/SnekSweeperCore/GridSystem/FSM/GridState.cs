using SnekSweeperCore.FSM;
using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem.FSM;

public abstract class GridState(GridStateMachine stateMachine) : IState
{
    protected GridStateMachine StateMachine { get; } = stateMachine;
    protected Grid Grid => StateMachine.Context.Grid;
    protected IHumbleGrid HumbleGrid => StateMachine.Context.HumbleGrid;
    protected GameRunRecorder RunRecorder => StateMachine.Context.RunRecorder;
    protected GridStateContext Context => StateMachine.Context;

    protected Task ChangeStateAsync(GridState newState, CancellationToken ct = default) =>
        StateMachine.ChangeStateAsync(newState, ct);

    public virtual Task OnEnterAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task OnExitAsync(CancellationToken ct = default) => Task.CompletedTask;

    public virtual Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) => Task.CompletedTask;
}

public abstract class Instantiated(GridStateMachine stateMachine)
    : GridState(stateMachine);