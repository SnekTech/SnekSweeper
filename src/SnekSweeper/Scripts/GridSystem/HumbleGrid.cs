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
using SnekSweeperCore.GridSystem.FSM;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.GridSystem;

[SceneTree]
public partial class HumbleGrid : Node2D, IHumbleGrid, ISceneScript
{
    readonly HUDEventBus _hudEventBus = EventBusOwner.HUDEventBus;

    Grid _grid = null!;
    GridStateMachine _gridStateMachine = null!;

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

    public void Init(Grid grid, GridStateMachine gridStateMachine) =>
        (_grid, _gridStateMachine) = (grid, gridStateMachine);

    public IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin)
    {
        var humbleCell = HumbleCell.InstantiateOnParent(this);
        humbleCell.OnInstantiate(gridIndex, gridSkin);
        return humbleCell;
    }

    public IEnumerable<IHumbleCell> HumbleCells => _grid.Cells.Select(cell => cell.HumbleCell);

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(HouseKeeper.ActivatedCheatCodeSet);

    public void LockStartIndexTo(GridIndex startIndex)
    {
        // todo: use actual implementation of locking
        MessageBox.Print($"start index has been locked to {startIndex}");
    }

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

        _gridStateMachine.HandleInputAsync(input, this.GetCancellationTokenOnTreeExit()).Fire();
    }
}