using GodotGadgets.Tasks;
using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
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
        var skin = HouseKeeper.MainSetting.CurrentSkinKey.ToSkin();
        TriggerPopulateExamples().Forget();
        return;

        async GDTaskVoid TriggerPopulateExamples()
        {
            var example1 = ExampleCard.InstantiateOnParent(ExampleCardContainer);
            await example1.InitAsync(_snapshot1, skin, this.GetCancellationTokenOnTreeExit());
        }
    }
}