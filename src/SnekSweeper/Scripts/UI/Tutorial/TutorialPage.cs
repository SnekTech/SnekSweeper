using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.GridSystem;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.UI.Tutorial;

[SceneTree]
public partial class TutorialPage : Control
{
    readonly GridSnapshot _snapshot1 = new(
        [
            [CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Covered, CellSnapshotState.Covered],
            [CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Flagged, CellSnapshotState.Covered],
            [CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Flagged, CellSnapshotState.Covered],
            [CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Revealed, CellSnapshotState.Flagged, CellSnapshotState.Covered],
            [CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered, CellSnapshotState.Covered],
        ],
        new[,]
        {
            {false,false,false,false,false},
            {false,false,false,true,false},
            {false,false,false,true,false},
            {false,false,false,true,false},
            {true,false,false,false,false},
        }
        );
    
    public override void _Ready()
    {
        var fromSnapshot = new FromGridSnapshot(_snapshot1, new RunStartInfo());
        var grid = CreateGrid(fromSnapshot);
        TheGrid.Init(grid);
        TriggerGridInitAsync(this.GetCancellationTokenOnTreeExit()).Forget();
        
        return;

        async GDTaskVoid TriggerGridInitAsync(CancellationToken ct = default)
        {
            await grid.InitCellsAsync(fromSnapshot.Snapshot.BombMatrix, ct);
            await grid.RestoreCellStatesAsync(fromSnapshot.Snapshot.SnapshotStates, ct);
        }

        Grid CreateGrid(LoadLevelSource loadLevelSource)
        {
            var gridSkin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
            return loadLevelSource.CreateGrid(TheGrid, EventBusOwner.GridEventBus, gridSkin);
        }

    }
}