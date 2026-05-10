using SnekSweeperCore.GameHistory;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class InstantiatedFromRecord(FromRunRecord fromRunRecord, GridStateMachine stateMachine)
    : Instantiated(stateMachine)
{
    readonly GameRunRecord _runRecord = fromRunRecord.RunRecord;
    
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        await Grid.InitCellsAsync(_runRecord.BombMatrix, ct);

        HumbleGrid.GridCursor.LockTo(_runRecord.StartIndex, Grid.Size);
    }

    public override Task OnExitAsync(CancellationToken ct = default)
    {
        HumbleGrid.GridCursor.Unlock();
        return Task.CompletedTask;
    }

    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        if (gridInput.Index != _runRecord.StartIndex) return;

        RunRecorder.MarkRunStartInfo(DateTime.Now, gridInput.Index);
        await ChangeStateAsync(new GameStart(StateMachine), ct);
        await StateMachine.HandleInputAsync(gridInput, ct);
    }
}