using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.GridSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class ExampleCard : HBoxContainer, ISceneScript
{
    public async GDTask InitAsync(GridSnapshot snapshot, GridSkin skin, CancellationToken ct = default)
    {
        var fromSnapshot = new FromGridSnapshot(snapshot, new RunStartInfo());
        var grid = fromSnapshot.CreateGrid(TheGrid, EventBusOwner.GridEventBus, skin);
        TheGrid.Init(grid);
        var gridSizePixels = new Vector2(grid.Size.Columns * HumbleCell.CellSizeInPixels, grid.Size.Rows * HumbleCell.CellSizeInPixels);
        GridParentMarker.Position = GetParentPosition(GridSubViewport.Size, gridSizePixels);
        
        await grid.InitCellsAsync(fromSnapshot.Snapshot.BombMatrix, ct);
        await grid.RestoreCellStatesAsync(fromSnapshot.Snapshot.SnapshotStates, ct);
    }

    static Vector2 GetParentPosition(Vector2 containerSize, Vector2 targetSize)
    {
        var center = containerSize / 2;
        return center - targetSize / 2;
    }
}
