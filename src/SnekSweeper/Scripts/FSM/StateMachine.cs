using System;

namespace SnekSweeper.FSM;

public abstract class StateMachine<TState, TContext>
    where TState : class, IState
{
    protected StateMachine(TContext context)
    {
        Context = context;
    }

    protected TState? CurrentState { get; private set; }
    public TContext Context { get; }

    protected void SetInitState(TState initState)
    {
        CurrentState = initState;
        CurrentState.OnEnter();
    }

    protected void ChangeState(TState newState)
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change from a null state");

        if (newState == CurrentState)
            return;

        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
}