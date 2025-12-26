using SnekSweeperCore.FSM;

namespace SnekSweeperCore.CellSystem.StateMachine;

public abstract class CellState(CellStateMachine stateMachine) : IState
{
    protected IHumbleCell HumbleCell => stateMachine.HumbleCell;

    private readonly HashSet<Transition> _transitions = [];

    public List<Transition> Transitions => _transitions.ToList();

    public void AddTransition(Transition transition) => _transitions.Add(transition);

    public virtual Task OnEnterAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public virtual Task OnExitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}