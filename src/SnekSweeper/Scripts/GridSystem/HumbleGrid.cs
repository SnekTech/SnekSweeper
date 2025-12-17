using GodotGadgets.Extensions;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.Commands;
using SnekSweeper.GameMode;
using SnekSweeper.GameSettings;
using SnekSweeper.GridSystem.Difficulty;
using SnekSweeper.GridSystem.LayMineStrategies;
using SnekSweeper.SkinSystem;
using SnekSweeper.UI;
using SnekSweeper.Widgets;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;
    readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    Grid _grid = null!;

    public CommandInvoker GridCommandInvoker { get; } = new();
    public Referee Referee { get; } = new();

    public override void _EnterTree()
    {
        var currentDifficulty = _mainSetting.CurrentDifficultyKey.ToDifficulty().DifficultyData;
        var currentStrategy = _mainSetting.CurrentStrategyKey.ToStrategy(currentDifficulty);
        _grid = new Grid(this, currentDifficulty.Size, currentStrategy);

        _hudEventBus.UndoPressed += OnUndoPressed;

        GridInputListener.PrimaryReleased += OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked += OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased += OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _hudEventBus.UndoPressed -= OnUndoPressed;

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
            var humbleCell = HumbleCell.InstantiateOnParent(this);
            humbleCells.Add(humbleCell);
            humbleCell.UseSkin(_mainSetting.CurrentSkinKey.ToSkin());
        }

        return humbleCells;
    }

    public IEnumerable<IHumbleCell> HumbleCells => _grid.Cells.Select(cell => cell.HumbleCell);

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects();

    void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        var shouldShowCursor = _grid.IsValidIndex(hoveringGridIndex);
        if (!shouldShowCursor)
        {
            Cursor.Hide();
            return;
        }

        Cursor.ShowAtHoveringCell(hoveringGridIndex);
    }

    void OnUndoPressed() => GridCommandInvoker.UndoCommandAsync().Fire();

    void OnPrimaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryReleasedAt(gridIndex).Fire();
    }

    void OnPrimaryDoubleClickedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnPrimaryDoubleClickedAt(gridIndex).Fire();
    }

    void OnSecondaryReleasedAt(GridIndex gridIndex)
    {
        if (!_grid.IsValidIndex(gridIndex))
            return;
        _grid.OnSecondaryReleasedAt(gridIndex).Fire();
    }
}