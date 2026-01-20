using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript
{
    IHumbleGrid HumbleGrid => _.GridStartPosition.Grid;

    public override void _ExitTree()
    {
        HouseKeeper.SaveCurrentPlayerData();
    }

    public async Task LoadLevelAsync(LoadLevelSource loadLevelSource, CancellationToken ct = default)
    {
        var grid = await loadLevelSource.CreateGridAsync(HumbleGrid, EventBusOwner.GridEventBus, ct);

        HumbleGrid.InitWithGrid(grid, new GridInitializer(loadLevelSource));
    }
}