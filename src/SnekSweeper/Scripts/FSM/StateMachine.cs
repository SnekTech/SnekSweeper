using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnekSweeper.FSM;

public abstract class StateMachine<TState>
    where TState : class, IState
{
    public bool IsAtState<T>() where T : TState
    {
        if (CurrentState == null) return false;

        return CurrentState == StateInstances[typeof(T)];
    }

    protected readonly Dictionary<Type, TState> StateInstances = new();
    protected TState? CurrentState { get; private set; }

    protected abstract void PopulateStateInstances();

    public async Task SetInitStateAsync<T>() where T : TState
    {
        PopulateStateInstances();

        if (StateInstances.Count == 0)
            throw new InvalidOperationException("cannot set initial state on a FSM with no state instances");

        CurrentState = StateInstances[typeof(T)];
        await CurrentState.OnEnterAsync();
    }

    public async Task ChangeStateAsync<T>() where T : TState
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change state from a null state");

        var newState = StateInstances[typeof(T)];
        if (newState == CurrentState)
            return;

        await CurrentState.OnExitAsync();
        CurrentState = newState;
        await CurrentState.OnEnterAsync();
    }
}