using System.Runtime.CompilerServices;
using SnekSweeperCore.FSM;
using SnekSweeperCore.GridSystem.FSM.States;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM;

public class GridStateMachine(LoadLevelSource loadLevelSource, Grid grid) : StateMachine<GridState>
{
    public Grid Grid => grid;
    
    protected override void SetupStateInstances()
    {
        var regularInstantiated = new RegularInstantiated(loadLevelSource, this);
        var fixedInstantiated = new FixedInstantiated(this);
        var gameStart = new GameStart(this);

        StateInstances[typeof(RegularInstantiated)] = regularInstantiated;
        StateInstances[typeof(FixedInstantiated)] = fixedInstantiated;
        StateInstances[typeof(GameStart)] = gameStart;
    }

    public async Task InitAsync(CancellationToken ct = default)
    {
        var setInitStateTask = loadLevelSource switch
        {
            RegularStart => SetInitStateAsync<RegularInstantiated>(ct),
            FromRunRecord => SetInitStateAsync<FixedInstantiated>(ct),
            _ => throw new SwitchExpressionException(),
        };

        await setInitStateTask;
    }

    public Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) =>
        CurrentState?.HandleInputAsync(gridInput, ct) ?? Task.CompletedTask;
}