using MemoryPack;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

public class CurrentRunInfo
{
    public GridSnapshot? GridSnapshot { get; set; }
}

[MemoryPackable]
public partial class GridSnapshot
{
    public required CellSnapshotState[][] SnapshotStates { get; init; }
}

public enum CellSnapshotState
{
    Covered = 0,
    Revealed,
    Flagged,
    Irrelevant,
}

public class GridSnapShotRecorder(CurrentRunInfo currentRunInfo)
{
    public void UpdateSnapshot(Grid grid)
    {
        currentRunInfo.GridSnapshot = grid.GetSnapshot();
    }
}

static class GridSnapshotExtensions
{
    extension(Grid grid)
    {
        public GridSnapshot GetSnapshot()
        {
            var (rows, columns) = grid.Size;
            var snapshotStates = new CellSnapshotState[rows, columns];
            foreach (var cell in grid.Cells)
            {
                var stateValue = cell switch
                {
                    { IsCovered: true } => CellSnapshotState.Covered,
                    { IsRevealed: true } => CellSnapshotState.Revealed,
                    { IsFlagged: true } => CellSnapshotState.Flagged,
                    _ => CellSnapshotState.Irrelevant,
                };
                snapshotStates.SetAt(cell.GridIndex, stateValue);
            }

            return new GridSnapshot { SnapshotStates = snapshotStates.ToJagged() };
        }
    }
}