using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.GameSettings;
using SnekSweeper.SkinSystem;
using SnekSweeper.UI;
using SnekSweeper.UI.GameResult;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.GridSystem.Difficulty;
using SnekSweeperCore.GridSystem.LayMineStrategies;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;
    readonly MainSetting _mainSetting = HouseKeeper.MainSetting;

    Grid _grid = null!;

    public CommandInvoker GridCommandInvoker { get; } = new();

    public Referee Referee { get; } = new(
        HouseKeeper.History,
        () =>
        {
            MessageBox.Print("You win!");
            Autoload.SceneSwitcher.GotoScene<WinningPage>();
        },
        () =>
        {
            MessageBox.Print("Game over! Bomb revealed!");
            Autoload.SceneSwitcher.GotoScene<LosingPage>();
        }
    );

    public override void _EnterTree()
    {
        var (size, currentStrategy, currentSkin) = ExtractGridInfoFromMainSettings(_mainSetting);
        var cells = CreateCells(size, currentSkin);
        _grid = new Grid(this, cells, currentStrategy, EventBusOwner.GridEventBus);

        _hudEventBus.UndoPressed += OnUndoPressed;

        GridInputListener.PrimaryReleased += OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked += OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased += OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
        return;

        static (GridSize size, ILayMineStrategy strategy, GridSkin gridSkin) ExtractGridInfoFromMainSettings(
            MainSetting mainSetting)
        {
            var currentDifficulty = mainSetting.CurrentDifficultyKey.ToDifficulty().DifficultyData;
            var currentStrategy = mainSetting.CurrentStrategyKey.ToStrategy(currentDifficulty);
            var currentSkin = mainSetting.CurrentSkinKey.ToSkin();
            return (currentDifficulty.Size, currentStrategy, currentSkin);
        }
    }

    public override void _ExitTree()
    {
        _hudEventBus.UndoPressed -= OnUndoPressed;

        GridInputListener.PrimaryReleased -= OnPrimaryReleasedAt;
        GridInputListener.PrimaryDoubleClicked -= OnPrimaryDoubleClickedAt;
        GridInputListener.SecondaryReleased -= OnSecondaryReleasedAt;
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
    }

    Cell[,] CreateCells(GridSize gridSize, GridSkin skin)
    {
        var cells = new Cell[gridSize.Rows, gridSize.Columns];
        foreach (var gridIndex in cells.Indices())
        {
            var humbleCell = HumbleCell.InstantiateOnParent(this);
            humbleCell.SetPosition(gridIndex);
            humbleCell.UseSkin(skin);

            var cell = new Cell(humbleCell, gridIndex);
            cells.SetAt(gridIndex, cell);
        }

        return cells;
    }

    public IEnumerable<IHumbleCell> HumbleCells => _grid.Cells.Select(cell => cell.HumbleCell);

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(HouseKeeper.ActivatedCheatCodeSet);

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