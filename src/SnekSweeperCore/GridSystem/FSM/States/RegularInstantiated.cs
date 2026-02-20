using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class RegularInstantiated(RegularStart regularStart, GridStateMachine stateMachine)
    : Instantiated(stateMachine)
{
    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput is PrimaryReleased primaryReleased)
        {
            var firstClickIndex = primaryReleased.Index;
            RunRecorder.MarkRunStartInfo(DateTime.Now, firstClickIndex);
            await Grid.InitCellsAsync(regularStart.LayMineFn(firstClickIndex), ct);
            await ChangeStateAsync(new GameStart(StateMachine), ct);
            await StateMachine.HandleInputAsync(gridInput, ct);
        }
    }
}