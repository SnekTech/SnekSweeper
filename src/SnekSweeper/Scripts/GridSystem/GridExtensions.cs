using SnekSweeper.CellSystem;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;

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

    extension(GridSize gridSize)
    {
        public Vector2 ToPixels(int cellSizePixels = HumbleCell.CellSizeInPixels)
        {
            var (rows, columns) = gridSize;
            return new Vector2(columns * cellSizePixels, rows * cellSizePixels);
        }
    }
}

static class GridSnapshotExtensions
{
    extension(Grid grid)
    {
        public void InitCells(GridSnapshot snapshot)
        {
            grid.InitCells(snapshot.BombMatrix);
            grid.RestoreCellStates(snapshot.SnapshotStates);
        }

        void RestoreCellStates(CellSnapshotState[,] snapshotStates)
        {
            foreach (var index in grid.Indices)
            {
                switch (snapshotStates.At(index))
                {
                    case CellSnapshotState.Revealed:
                        grid.ApplyCommand(index, new CellCommand.RevealCover());
                        break;
                    case CellSnapshotState.Flagged:
                        grid.ApplyCommand(index, new CellCommand.ToggleFlag());
                        break;
                }
            }
        }
    }
}
