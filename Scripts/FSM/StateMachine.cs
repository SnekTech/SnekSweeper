using System;

namespace SnekSweeper.FSM;

public class StateMachine<TState, TContext>
    where TState : class, IState
{
    protected StateMachine(TContext context)
    {
        Context = context;
    }

    public TState CurrentState { get; private set; }
    public TContext Context { get; }

    public void SetInitState(TState initState)
    {
        CurrentState = initState;
        CurrentState.OnEnter();
    }

    public void ChangeState(TState newState)
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