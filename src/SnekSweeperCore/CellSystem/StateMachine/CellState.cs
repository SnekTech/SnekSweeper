using SnekSweeperCore.FSM;

namespace SnekSweeperCore.CellSystem.StateMachine;

public abstract class CellState(CellStateMachine stateMachine) : IState
{
    protected IHumbleCell HumbleCell => stateMachine.HumbleCell;

    readonly HashSet<Transition> _transitions = [];

    public IEnumerable<Transition> Transitions => _transitions;

    public void SetTransitions(IEnumerable<Transition> transitions) => _transitions.UnionWith(transitions);

    public virtual Task OnEnterAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public virtual Task OnExitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public readonly record struct Transition(CellState To, CellRequest Request);