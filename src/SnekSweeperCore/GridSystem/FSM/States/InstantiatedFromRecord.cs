using SnekSweeperCore.GameHistory;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class InstantiatedFromRecord(GameRunRecord runRecord, GridStateMachine stateMachine)
    : Instantiated(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        await Grid.InitCellsAsync(runRecord.StartIndex, runRecord.BombMatrix, ct);

        HumbleGrid.LockStartIndexTo(runRecord.StartIndex);
    }

    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput.Index != runRecord.StartIndex) return;

        await ChangeStateAsync<GameStart>(ct);
        await Grid.HandleInputAsync(gridInput, ct);
    }
}