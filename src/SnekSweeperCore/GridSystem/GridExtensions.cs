using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public static class GridExtensions
{
    extension(Grid)
    {
        public static Grid Create(IHumbleGrid humbleGrid, GridSize gridSize, GridSkin skin, GridEventBus eventBus,
            CommandInvoker commandInvoker)
        {
            // todo: should humbleGrid be passed here? Grid constructor does not need it.
            var cells = Grid.CreateCells(humbleGrid, gridSize, skin);
            return new Grid(cells, eventBus, commandInvoker);
        }

        static Cell[,] CreateCells(IHumbleGrid humbleGrid, GridSize gridSize, GridSkin skin) =>
            MatrixExtensions.Create(gridSize,
                gridIndex =>
                {
                    // todo: consider refactor this, humbleGrid only used to instantiate humbleCells
                    // maybe use a factory Func<HumbleCell> instead?
                    var humbleCell = humbleGrid.HumbleCellsContainer.InstantiateHumbleCell(gridIndex, skin);
                    return new Cell(humbleCell, gridIndex);
                });
    }
}