using MemoryPack;
using SnekSweeperCore.GameMode;

namespace SnekSweeperCore.GridSystem;

public enum CellSnapshotState
{
    Covered = 0,
    Revealed,
    Flagged,
    Irrelevant,
}

[MemoryPackable]
public partial record GridSnapshot(
    CellSnapshotState[][] SnapshotStates,
    bool[,] BombMatrix
);

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