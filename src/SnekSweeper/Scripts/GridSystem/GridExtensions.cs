using GodotTask;
using SnekSweeper.CellSystem;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.GridSystem;

static class GridIndexExtensions
{
    extension(GridIndex index)
    {
        public Vector2 ToPosition(int cellSizePixels = HumbleCell.CellSizeInPixels)
        {
            var (i, j) = index;
            return new Vector2(j * cellSizePixels, i * cellSizePixels);
        }
    }
}

static class GridSnapshotExtensions
{
    extension(Grid grid)
    {
        public async GDTask RestoreCellStatesAsync(CellSnapshotState[][] snapshotStates, CancellationToken ct = default)
        {
            var cellStatesMatrix = MatrixExtensions.FromJagged(snapshotStates);

            var tasks = grid.Cells
                .Select(cell => (cell, state: cellStatesMatrix.At(cell.GridIndex)))
                .Select(tuple => tuple.state switch
                {
                    CellSnapshotState.Revealed => tuple.cell.RevealAsync(ct).AsGDTask(),
                    CellSnapshotState.Flagged => tuple.cell.SwitchFlagAsync(ct).AsGDTask(),
                    _ => GDTask.CompletedTask,
                });

            await GDTask.WhenAll(tasks);
        }
    }
}