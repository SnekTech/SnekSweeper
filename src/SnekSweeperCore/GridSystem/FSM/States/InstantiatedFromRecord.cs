using SnekSweeperCore.GameHistory;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class InstantiatedFromRecord(GameRunRecord runRecord, GridStateMachine stateMachine)
    : Instantiated(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        await Grid.InitCellsAsync(runRecord.BombMatrix, ct);

        HumbleGrid.GridCursor.LockTo(runRecord.StartIndex, Grid.Size);
    }

    public override Task OnExitAsync(CancellationToken ct = default)
    {
        HumbleGrid.GridCursor.Unlock();
        return Task.CompletedTask;
    }

    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput.Index != runRecord.StartIndex) return;

        RunRecorder.MarkRunStartInfo(DateTime.Now, gridInput.Index);
        await ChangeStateAsync<GameStart>(ct);
        await StateMachine.HandleInputAsync(gridInput, ct);
    }
}