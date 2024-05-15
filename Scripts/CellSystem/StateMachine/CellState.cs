using SnekSweeper.FSM;

namespace SnekSweeper.CellSystem.StateMachine;

public abstract class CellState : IState
{
    protected readonly CellStateMachine StateMachine;
    protected Cell Cell => StateMachine.Context;

    protected CellState(CellStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract CellStateValue Value { get; }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnPrimaryReleased()
    {
    }

    public virtual void OnSecondaryReleased()
    {
    }

    public virtual void OnPrimaryDouble()
    {
    }
}