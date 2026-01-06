using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeperCore.GridSystem;

public interface IHumbleGrid
{
    CommandInvoker GridCommandInvoker { get; }
    IEnumerable<IHumbleCell> HumbleCells { get; }
    Referee Referee { get; }
    void TriggerInitEffects();
    IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin);
    void InitWithGrid(Grid grid, GridInitializer gridInitializer);
}