using SnekSweeperCore.CellSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public static class GridExtensions
{
    extension(Grid)
    {
        public static Grid Create(IHumbleGrid humbleGrid, GridSize gridSize, GridSkin skin, GridEventBus eventBus)
        {
            var cells = Grid.CreateCells(humbleGrid, gridSize, skin);
            return new Grid(humbleGrid, cells, eventBus);
        }
        
        static Cell[,] CreateCells(IHumbleGrid humbleGrid, GridSize gridSize, GridSkin skin) =>
            MatrixExtensions.Create(gridSize,
                gridIndex =>
                {
                    var humbleCell = humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, skin);
                    return new Cell(humbleCell, gridIndex);
                });
    }
}