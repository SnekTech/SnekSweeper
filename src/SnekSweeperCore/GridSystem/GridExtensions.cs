using SnekSweeperCore.Commands;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public static class GridExtensions
{
    extension(Grid)
    {
        public static Grid Create(IHumbleCellsContainer cellsContainer, GridSize gridSize, GridSkin skin, GridEventBus eventBus,
            CommandInvoker commandInvoker)
        {
            var cells = MatrixExtensions.Create(gridSize, gridIndex => cellsContainer.InstantiateCell(gridIndex, skin));
            return new Grid(cells, eventBus, commandInvoker);
        }
    }
}
