using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class ExampleCard : HBoxContainer, ISceneScript
{
    public async GDTask InitAsync(GridSnapshot snapshot, GridSkin skin, CancellationToken ct = default)
    {
        var grid = Grid.Create(TheGrid, snapshot.BombMatrix.Size, skin, EventBusOwner.GridEventBus);
        TheGrid.Init(grid);
        
        GridParentMarker.Position = GetParentPosition(GridSubViewport.Size, grid.Size.ToPixels());

        await grid.InitCellsAsync(snapshot, ct);
    }

    static Vector2 GetParentPosition(Vector2 containerSize, Vector2 targetSize)
    {
        var center = containerSize / 2;
        return center - targetSize / 2;
    }
}
