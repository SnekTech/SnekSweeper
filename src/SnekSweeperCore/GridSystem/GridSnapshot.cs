using MemoryPack;
using SnekSweeperCore.CellSystem;

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
    CellSnapshotState[,] SnapshotStates,
    bool[,] BombMatrix
);

public static class GridSnapshotExtensions
{
    extension(Grid grid)
    {
        public GridSnapshot GetSnapshot()
        {
            var (rows, columns) = grid.Size;
            var snapshotStates = new CellSnapshotState[rows, columns];
            foreach (var index in grid.Indices)
            {
                snapshotStates.SetAt(index, grid.StateAt(index) switch
                {
                    CellState.Covered => CellSnapshotState.Covered,
                    CellState.Revealed => CellSnapshotState.Revealed,
                    CellState.Flagged => CellSnapshotState.Flagged,
                    _ => CellSnapshotState.Irrelevant,
                });
            }

            return new GridSnapshot(snapshotStates, grid.BombMatrix);
        }
    }
}
