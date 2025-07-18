﻿namespace SnekSweeper.FSM;

public abstract class StateMachine<TState>
    where TState : class, IState
{
    public bool IsAtState<T>() where T : TState
    {
        if (CurrentState == null) return false;

        return CurrentState == GetState<T>();
    }

    protected TState GetState<T>() => StateInstances[typeof(T)];

    protected readonly Dictionary<Type, TState> StateInstances = [];
    protected TState? CurrentState { get; private set; }
    private bool _isTransitioning;

    protected abstract void SetupStateInstances();

    public async Task SetInitStateAsync<T>() where T : TState
    {
        SetupStateInstances();

        if (StateInstances.Count == 0)
            throw new InvalidOperationException("cannot set initial state on a FSM with no state instances");

        _isTransitioning = true;
        CurrentState = GetState<T>();
        await CurrentState.OnEnterAsync();
        _isTransitioning = false;
    }

    protected async Task ChangeStateAsync(TState newState)
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change state from a null state");
        if (_isTransitioning)
            return;

        if (newState == CurrentState)
            return;

        _isTransitioning = true;
        await CurrentState.OnExitAsync();
        CurrentState = newState;
        await CurrentState.OnEnterAsync();
        _isTransitioning = false;
    }
}