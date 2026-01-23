using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.LevelManagement;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript
{
    HumbleGrid HumbleGrid => _.GridStartPosition.Grid;

    public override void _ExitTree()
    {
        // todo: move this to grid FSM
        HouseKeeper.SaveCurrentPlayerData();
    }

    public async Task LoadLevelAsync(LoadLevelSource loadLevelSource, CancellationToken ct = default)
    {
        var grid = loadLevelSource.CreateGrid(HumbleGrid, EventBusOwner.GridEventBus);
        var gridStateMachine = new GridStateMachine(loadLevelSource, grid);
        await gridStateMachine.InitAsync(ct);

        HumbleGrid.Init(grid, gridStateMachine);
    }
}