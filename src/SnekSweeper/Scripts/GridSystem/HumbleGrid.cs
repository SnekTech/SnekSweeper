using GodotGadgets.Tasks;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.UI;
using SnekSweeper.UI.GameResult;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
using SnekSweeperCore.GameMode;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.LevelManagement;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    Grid _grid = null!;
    GridInitializer _gridInitializer = null!;

    public CommandInvoker GridCommandInvoker { get; } = new();

    public Referee Referee { get; } = new(
        HouseKeeper.History,
        () =>
        {
            MessageBox.Print("You win!");
            Autoload.SceneSwitcher.GotoSceneAsync<WinningPage>().Fire();
        },
        () =>
        {
            MessageBox.Print("Game over! Bomb revealed!");
            Autoload.SceneSwitcher.GotoSceneAsync<LosingPage>().Fire();
        }
    );

    public override void _EnterTree()
    {
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

    public void InitWithGrid(Grid grid, GridInitializer gridInitializer) =>
        (_grid, _gridInitializer) = (grid, gridInitializer);

    public IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin)
    {
        var humbleCell = HumbleCell.InstantiateOnParent(this);
        humbleCell.SetPosition(gridIndex);
        humbleCell.UseSkin(gridSkin);
        return humbleCell;
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

        HandleAsync().Fire();
        return;

        async Task HandleAsync()
        {
            await _gridInitializer.TryHandleFirstPrimaryClickAsync(_grid, gridIndex,
                this.GetCancellationTokenOnTreeExit());
            await _grid.OnPrimaryReleasedAt(gridIndex, this.GetCancellationTokenOnTreeExit());
        }
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