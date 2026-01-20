using System.Runtime.CompilerServices;
using SnekSweeperCore.FSM;
using SnekSweeperCore.GridSystem.FSM.States;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM;

public class GridStateMachine(LoadLevelSource loadLevelSource) : StateMachine<GridState>
{
    protected override void SetupStateInstances()
    {
        var regularInstantiated = new RegularInstantiated(this);
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
}