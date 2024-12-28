using System;
using System.Collections.Generic;

namespace SnekSweeper.FSM;

public abstract class StateMachine<TState, TContext>
    where TState : class, IState
{
    protected StateMachine(TContext context)
    {
        Context = context;
    }

    public TContext Context { get; }

    public bool IsAtState<T>() where T : TState
    {
        if (CurrentState == null) return false;

        return CurrentState == StateInstances[typeof(T)];
    }

    protected readonly Dictionary<Type, TState> StateInstances = new();
    protected TState? CurrentState { get; private set; }

    public void SetInitState<T>() where T : TState
    {
        if (StateInstances.Count == 0)
            throw new InvalidOperationException("set initial state before populate the state instance dictionary");
        CurrentState = StateInstances[typeof(T)];
        CurrentState.OnEnter();
    }

    public void ChangeState<T>() where T : TState
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change state from a null state");

        var newState = StateInstances[typeof(T)];
        if (newState == CurrentState)
            return;

        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
}