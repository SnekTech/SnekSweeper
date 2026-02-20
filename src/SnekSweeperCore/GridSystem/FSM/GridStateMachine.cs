using System.Runtime.CompilerServices;
using SnekSweeperCore.FSM;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem.FSM.States;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM;

public record GridStateContext(
    Grid Grid,
    IHumbleGrid HumbleGrid,
    GameRunRecorder RunRecorder,
    ILevelOrchestrator LevelOrchestrator
);

public class GridStateMachine(GridStateContext context) : StateMachineV2<GridState>
{
    public GridStateContext Context => context;

    public Task InitAsync(LoadLevelSource loadLevelSource,CancellationToken ct = default)
    {
        Instantiated instantiated = loadLevelSource switch
        {
            RegularStart regularStart => new RegularInstantiated(regularStart, this),
            FromRunRecord fromRunRecord => new InstantiatedFromRecord(fromRunRecord.RunRecord, this),
            _ => throw new SwitchExpressionException(),
        };
        return SetInitStateAsync(instantiated, ct);
    }

    public Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default) =>
        CurrentState?.HandleInputAsync(gridInput, ct) ?? Task.CompletedTask;
}