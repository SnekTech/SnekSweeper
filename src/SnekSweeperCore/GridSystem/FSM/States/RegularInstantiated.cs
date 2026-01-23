using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class RegularInstantiated(LoadLevelSource loadLevelSource, GridStateMachine stateMachine)
    : GridState(stateMachine)
{
    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput is PrimaryReleased primaryReleased)
        {
            var firstClickIndex = primaryReleased.Index;
            await Grid.InitCellsAsync(firstClickIndex, loadLevelSource.LayMineFn(firstClickIndex), ct);
            await ChangeStateAsync<GameStart>(ct);
            await Grid.HandleInputAsync(gridInput, ct);
        }
    }
}