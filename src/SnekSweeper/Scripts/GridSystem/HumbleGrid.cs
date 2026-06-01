using GodotTask;
using SnekSweeper.Autoloads;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.UI.Level;
using SnekSweeper.Widgets;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    Grid _grid = null!;

    public CommandInvoker GridCommandInvoker { get; } = new();

    public override void _EnterTree()
    {
        _hudEventBus.UndoPressed += OnUndoPressed;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _hudEventBus.UndoPressed -= OnUndoPressed;
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
    }

    public void Init(Grid grid) => _grid = grid;

    public IHumbleCellsContainer HumbleCellsContainer => CellsContainer;
    public IGridCursor GridCursor => Cursor;

    public void PlayCongratulationEffects() => CellsContainer.PlayShuffleEffect();

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(HouseKeeper.ActivatedCheatCodeSet);

    void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        Cursor.ShowAt(hoveringGridIndex, _grid.Size);
    }

    void OnUndoPressed() => GridCommandInvoker.UndoCommandAsync().AsGDTask().Forget();
}