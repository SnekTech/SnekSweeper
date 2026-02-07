using GodotGadgets.Tasks;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Enums;
using GTweens.Tweens;
using GTweensGodot.Extensions;
using SnekSweeper.Autoloads;
using SnekSweeper.CellSystem;
using SnekSweeper.CheatCodeSystem;
using SnekSweeper.UI;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.Commands;
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
    readonly List<HumbleCell> _humbleCells = [];

    readonly HashSet<GTween> _tweens = [];

    public CommandInvoker GridCommandInvoker { get; } = new();

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

        foreach (var tween in _tweens)
        {
            tween.Kill();
        }
    }

    public void Init(Grid grid, GridStateMachine gridStateMachine) =>
        (_grid, _gridStateMachine) = (grid, gridStateMachine);

    public IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin)
    {
        var humbleCell = HumbleCell.InstantiateOnParent(CellsContainer);
        humbleCell.OnInstantiate(gridIndex, gridSkin);
        _humbleCells.Add(humbleCell);
        return humbleCell;
    }

    public IEnumerable<IHumbleCell> HumbleCells => _humbleCells.ToList();

    public IGridCursor GridCursor => Cursor;

    public void PlayCongratulationEffects()
    {
        const float duration = 0.1f;

        var shuffleTween = CreateShuffleTween();
        _tweens.Add(shuffleTween);
        shuffleTween.Play();

        return;

        GTween CreateShuffleTween()
        {
            var shuffleTweenBuilder = GTweenSequenceBuilder.New();
            foreach (var humbleCell in _humbleCells)
            {
                var singleCellShuffle = humbleCell.TweenPosition(Vector2.Zero, duration)
                    .SetEasing(Easing.OutQuint);
                shuffleTweenBuilder
                    .Append(singleCellShuffle);
            }
            var tween = shuffleTweenBuilder.Build()
                .SetMaxLoops(ResetMode.PingPong);
            return tween;
        }
    }

    public void TriggerInitEffects() => this.TriggerCheatCodeInitEffects(HouseKeeper.ActivatedCheatCodeSet);

    void OnHoveringGridIndexChanged(GridIndex hoveringGridIndex)
    {
        Cursor.ShowAt(hoveringGridIndex, _grid.Size);
    }

    void OnUndoPressed() => GridCommandInvoker.UndoCommandAsync().Fire();

    void OnGridInputEmitted(GridInput input)
    {
        _gridStateMachine.HandleInputAsync(input, this.GetCancellationTokenOnTreeExit()).Fire();
    }
}