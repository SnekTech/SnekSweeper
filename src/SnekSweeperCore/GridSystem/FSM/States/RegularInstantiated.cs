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
            await Grid.InitCellsAsync(firstClickIndex, regularStart.LayMineFn(firstClickIndex), ct);
            await ChangeStateAsync<GameStart>(ct);
            await Grid.HandleInputAsync(gridInput, ct);
        }
    }
}