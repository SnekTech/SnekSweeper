using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public abstract class CellState(CellStateMachine stateMachine) : IState
{
    protected Cell Cell => stateMachine.Cell;

    private readonly HashSet<Transition> _transitions = [];

    public List<Transition> Transitions => _transitions.ToList();

    public void AddTransition(Transition transition) => _transitions.Add(transition);

    public virtual Task OnEnterAsync() => Task.CompletedTask;

    public virtual Task OnExitAsync() => Task.CompletedTask;
}