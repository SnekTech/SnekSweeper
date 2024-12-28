using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public abstract class CellState : IState
{
    private readonly CellStateMachine stateMachine;
    protected Cell Cell => stateMachine.Cell;

    protected CellState(CellStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }


    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void Reveal()
    {
    }

    public virtual void SwitchFlag()
    {
    }

    protected void ChangeState<T>() where T : CellState
    {
        stateMachine.ChangeState<T>();
    }
}