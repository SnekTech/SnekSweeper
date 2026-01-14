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
            // BUG: AddChild() failed, 'cause the parent is busy
            // 可能和赢两次有关
            Autoload.SceneSwitcher.GotoSceneAsync<LosingPage>().Fire();
        }
    );

    public override void _EnterTree()
    {
        _hudEventBus.UndoPressed += OnUndoPressed;
        GridInputListener.GridInputEmitted += OnGridInputEmitted;
        GridInputListener.HoveringGridIndexChanged += OnHoveringGridIndexChanged;
    }

    public override void _ExitTree()
    {
        _hudEventBus.UndoPressed -= OnUndoPressed;
        GridInputListener.GridInputEmitted -= OnGridInputEmitted;
        GridInputListener.HoveringGridIndexChanged -= OnHoveringGridIndexChanged;
    }

    public void InitWithGrid(Grid grid, GridInitializer gridInitializer) =>
        (_grid, _gridInitializer) = (grid, gridInitializer);

    public IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin)
    {
        var humbleCell = HumbleCell.InstantiateOnParent(this);
        humbleCell.OnInstantiate(gridIndex, gridSkin);
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

    void OnGridInputEmitted(GridInput input)
    {
        if (!_grid.IsValidIndex(input.Index)) return;

        var tokenOnDestroy = this.GetCancellationTokenOnTreeExit();
        var handleInputTask = input switch
        {
            PrimaryReleased primaryReleased => HandlePrimaryReleasedAsync(primaryReleased, tokenOnDestroy),
            _ => _grid.HandleInputAsync(input, tokenOnDestroy),
        };
        handleInputTask.Fire();
    }

    async Task HandlePrimaryReleasedAsync(PrimaryReleased primaryReleased,
        CancellationToken cancellationToken = default)
    {
        await _gridInitializer.TryHandleFirstPrimaryClickAsync(_grid, primaryReleased.Index, cancellationToken);
        await _grid.HandleInputAsync(primaryReleased, cancellationToken);
    }
}