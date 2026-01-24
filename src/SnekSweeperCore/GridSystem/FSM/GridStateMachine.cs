using System.Runtime.CompilerServices;
using SnekSweeperCore.FSM;
using SnekSweeperCore.GridSystem.FSM.States;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM;

public record GridStateContext(Grid Grid, IHumbleGrid HumbleGrid);

public class GridStateMachine(LoadLevelSource loadLevelSource, GridStateContext context) : StateMachine<GridState>
{
    public GridStateContext Context => context;

    protected override void SetupStateInstances()
    {
        Instantiated instantiated = loadLevelSource switch
        {
            RegularStart regularStart => new RegularInstantiated(regularStart, this),
            FromRunRecord fromRunRecord => new InstantiatedFromRecord(fromRunRecord.RunRecord, this),
            _ => throw new SwitchExpressionException(),
        };

        var gameStart = new GameStart(this);

        StateInstances[typeof(Instantiated)] = instantiated;
        StateInstances[typeof(GameStart)] = gameStart;
    }

    public Task InitAsync(CancellationToken ct = default) => SetInitStateAsync<Instantiated>(ct);

    public Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) =>
        CurrentState?.HandleInputAsync(gridInput, ct) ?? Task.CompletedTask;
}