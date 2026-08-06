using SnekSweeper.Autoloads;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.GridSystem;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    // todo: remove this 2-way-reference between GridLogic
    Grid _grid = null!;

    public override void _EnterTree()
    {
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
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
}