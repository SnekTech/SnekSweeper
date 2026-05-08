using MemoryPack;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;

namespace SnekSweeperCore.LevelManagement;

public class CurrentRunInfo
{
    public GridSnapshot? GridSnapshot { get; set; }
}

[MemoryPackable]
public partial record GridSnapshot(
    CellSnapshotState[][] SnapshotStates,
    bool[,] BombMatrix
);

public enum CellSnapshotState
{
    Covered = 0,
    Revealed,
    Flagged,
    Irrelevant,
}

public class GridSnapShotRecorder(CurrentRunInfo currentRunInfo)
{
    public void UpdateGridSnapshot(Grid grid)
    {
        currentRunInfo.GridSnapshot = grid.GetSnapshot();
    }

    public void ClearSnapshot() => currentRunInfo.GridSnapshot = null;
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

            return new GridSnapshot(snapshotStates.ToJagged(), grid.BombMatrix);
        }
    }
}