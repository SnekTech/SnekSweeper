using GodotGadgets.Extensions;
using GodotGadgets.Tasks;
using GodotGadgets.TweenStuff;
using GodotTask;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Enums;
using GTweens.Tweens;
using GTweensGodot.Extensions;
using SnekSweeper.CellSystem;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem;
using SnekSweeperCore.GridSystem;
using SnekSweeperCore.SkinSystem;

namespace SnekSweeper.GridSystem;

public partial class HumbleCellsContainer : Node2D, ICellRenderer, IHumbleCellCollection
{
    readonly Dictionary<GridIndex, HumbleCell> _cells = [];
    CancellationTokenSource? _celebrationCts;

    public IEnumerable<IHumbleCell> HumbleCells => _cells.Values;

    public override void _ExitTree()
    {
        StopCelebration();
    }

    public void InstantiateCells(GridSize gridSize, GridSkin skin)
    {
        Clear();

        foreach (var index in gridSize.Indices())
        {
            var cell = HumbleCell.InstantiateOnParent(this);
            cell.SetUpAt(index, skin);
            _cells.Add(index, cell);
        }
    }

    public IHumbleCell CellAt(GridIndex index) => _cells[index];

    public void Render(CellOutcome outcome) => _cells[outcome.Info.Index].Render(outcome);

    public void Clear()
    {
        this.ClearChildren();
        _cells.Clear();
        StopCelebration();
    }

    public void PlayShuffleEffect()
    {
        StopCelebration();
        _celebrationCts = new CancellationTokenSource();

        var tween = BuildShuffleTween();
        tween.PlayAsyncGD(_celebrationCts.Token).Forget();
        return;

        GTween BuildShuffleTween()
        {
            const float duration = 0.1f;

            var builder = _cells.Values
                .Select(cell => cell.TweenPosition(Vector2.Zero, duration).SetEasing(Easing.OutQuint))
                .Aggregate(GTweenSequenceBuilder.New(), (builder, nextTween) => builder.Append(nextTween));

            return builder.Build().SetMaxLoops(ResetMode.PingPong);
        }
    }

    void StopCelebration()
    {
        _celebrationCts?.CancelAndDispose();
        _celebrationCts = null;
    }
}