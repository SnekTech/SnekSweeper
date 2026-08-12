using SnekSweeper.Autoloads;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
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

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(HouseKeeper.ActivatedCheatCodeSet);

    void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        Cursor.ShowAt(hoveringGridIndex, _gridSize);
    }
}
