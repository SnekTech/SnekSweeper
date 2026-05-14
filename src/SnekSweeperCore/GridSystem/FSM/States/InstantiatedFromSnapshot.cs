using SnekSweeperCore.LevelManagement;

namespace SnekSweeperCore.GridSystem.FSM.States;

public sealed class InstantiatedFromSnapshot(FromGridSnapshot fromGridSnapshot, GridStateMachine stateMachine)
    : Instantiated(stateMachine)
{
    public override async Task OnEnterAsync(CancellationToken ct = default)
    {
        var (cellSnapshotStates, bombs) = fromGridSnapshot.Snapshot;
        var cellStatesMatrix = MatrixExtensions.FromJagged(cellSnapshotStates);

        await Grid.InitCellsAsync(bombs, ct);
        await RestoreCellStatesAsync();
        RunRecorder.MarkRunStartInfo(fromGridSnapshot.StartInfo);
        return;

        async Task RestoreCellStatesAsync()
        {
            var tasks = Grid.Cells
                .Select(cell => (cell, state: cellStatesMatrix.At(cell.GridIndex)))
                .Select(tuple => tuple.state switch
                {
                    CellSnapshotState.Revealed => tuple.cell.RevealAsync(ct),
                    CellSnapshotState.Flagged => tuple.cell.SwitchFlagAsync(ct),
                    _ => Task.CompletedTask,
                });

            await Task.WhenAll(tasks);
        }
    }

    public override async Task HandleInputAsync(GridInput gridInput, CancellationToken ct = default)
    {
        /*
         * cannot change state during entering or exiting,
         * so change to game start on the first grid input
         */
        await ChangeStateAsync(new GameStart(StateMachine), ct);
        await StateMachine.HandleInputAsync(gridInput, ct);
    }
}