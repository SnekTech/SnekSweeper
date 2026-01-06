using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.Levels;

[SceneTree]
public partial class Level1 : Node2D, ISceneScript
{
    IHumbleGrid HumbleGrid => _.GridStartPosition.Grid;

    public override void _ExitTree()
    {
        HouseKeeper.SaveCurrentPlayerData();
    }

    public void LoadLevel(LoadLevelSource loadLevelSource)
    {
        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        var grid = loadLevelSource.CreateGrid(HumbleGrid, EventBusOwner.GridEventBus, skin);
        HumbleGrid.InitWithGrid(grid, new GridInitializer(loadLevelSource));
    }
}