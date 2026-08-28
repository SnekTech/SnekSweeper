using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SaveLoad;

namespace SnekSweeper.GridSystem;

[Meta(typeof(IAutoNode))]
[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    ISaveDataStore SaveData => this.DependOn<ISaveDataStore>();

    GridSize _gridSize;

    public override void _EnterTree()
    {
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
    }

    public void Init(GridSize gridSize) => _gridSize = gridSize;

    public IHumbleCellCollection HumbleCellsContainer => CellsContainer;
    public ICellFactory CellFactory => CellsContainer;
    public IGridCursor GridCursor => Cursor;

    public void PlayCongratulationEffects() => CellsContainer.PlayShuffleEffect();

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(SaveData.State.ActivatedCheatCodeSet);

    void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        Cursor.ShowAt(hoveringGridIndex, _gridSize);
    }
}
