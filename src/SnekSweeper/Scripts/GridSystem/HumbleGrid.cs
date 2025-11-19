using GodotGadgets.Extensions;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.GridSystem.LayMineStrategies;
using SnekSweeper.UI;
using SnekSweeper.Widgets;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    private readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;
    private readonly GridEventBus _gridEventBus = EventBusOwner.GridEventBus;
    private readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    private Grid _grid = null!;
    private Referee _referee = null!;

    public CommandInvoker GridCommandInvoker { get; } = new();

    public override void _Ready()
    {
        var currentDifficulty = _mainSetting.CurrentDifficulty.DifficultyData;
        var currentStrategy = _mainSetting.CurrentStrategyName.ToStrategy(currentDifficulty);
        _grid = new Grid(this, currentDifficulty.Size, currentStrategy);
        _referee = new Referee(_grid);
    }

    public override void _EnterTree()
    {
        _hudEventBus.UndoPressed += OnUndoPressed;
        _gridEventBus.InitCompleted += OnGridInitCompleted;

        GridInputListener.PrimaryReleased += OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked += OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased += OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _referee.Dispose();

        _hudEventBus.UndoPressed -= OnUndoPressed;
        _gridEventBus.InitCompleted -= OnGridInitCompleted;

        GridInputListener.PrimaryReleased -= OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked -= OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased -= OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
    }

    public List<IHumbleCell> InstantiateHumbleCells(int count)
    {
        var humbleCells = new List<IHumbleCell>();

        for (var i = 0; i < count; i++)
        {
            var humbleCell = SceneFactory.Instantiate<HumbleCell>();
            humbleCells.Add(humbleCell);
            humbleCell.UseSkin(_mainSetting.CurrentSkin);
            AddChild(humbleCell);
        }

        return humbleCells;
    }

    public IEnumerable<IHumbleCell> HumbleCells => _grid.Cells.Select(cell => cell.HumbleCell);

    private void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        var shouldShowCursor = _grid.IsValidIndex(hoveringGridIndex);
        if (!shouldShowCursor)
        {
            Cursor.Hide();
            return;
        }

        Cursor.ShowAtHoveringCell(hoveringGridIndex);
    }

    private void OnUndoPressed() => GridCommandInvoker.UndoCommandAsync().Fire();

    private void OnGridInitCompleted() => this.TriggerCheatCodeInitEffects();

    private void OnPrimaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryReleasedAt(gridIndex).Fire();
    }

    private void OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryDoubleClickedAt(gridIndex).Fire();
    }

    private void OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnSecondaryReleasedAt(gridIndex).Fire();
    }
}