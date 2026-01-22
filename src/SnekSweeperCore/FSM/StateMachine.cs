namespace SnekSweeperCore.FSM;

public abstract class StateMachine<TState>
    where TState : class, IState
{
    public bool IsAtState<T>() where T : TState
    {
        if (CurrentState == null) return false;

        return CurrentState == GetState<T>();
    }

    TState GetState<T>() => StateInstances[typeof(T)];

    protected readonly Dictionary<Type, TState> StateInstances = [];
    protected TState? CurrentState { get; private set; }
    bool HasInitialized => CurrentState is not null;
    bool _isTransitioning;

    protected abstract void SetupStateInstances();

    public async Task SetInitStateAsync<T>(CancellationToken ct = default) where T : TState
    {
        if (HasInitialized) return;
        
        SetupStateInstances();

        if (StateInstances.Count == 0)
            throw new InvalidOperationException("cannot set initial state on a FSM with no state instances");

        _isTransitioning = true;
        CurrentState = GetState<T>();
        await CurrentState.OnEnterAsync(ct);
        _isTransitioning = false;
    }

    protected async Task ChangeStateAsync(TState newState, CancellationToken ct = default)
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change state from a null state");
        if (_isTransitioning)
            return;

        if (newState == CurrentState)
            return;

        _isTransitioning = true;
        await CurrentState.OnExitAsync(ct);
        CurrentState = newState;
        await CurrentState.OnEnterAsync(ct);
        _isTransitioning = false;
    }

    public Task ChangeStateAsync<T>(CancellationToken ct = default) where T : TState =>
        ChangeStateAsync(GetState<T>(), ct);
}