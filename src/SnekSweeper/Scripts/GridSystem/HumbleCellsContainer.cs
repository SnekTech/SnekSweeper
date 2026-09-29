using GodotGadgets.Extensions;
using GodotGadgets.TweenStuff;
using GodotTask;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Enums;
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

    public IEnumerable<IHumbleCell> HumbleCells => _cells.Values;

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

    public void Render(CellInfo info, CellOutcome outcome) => _cells[info.Index].Render(info, outcome);

    public void Clear()
    {
        this.ClearChildren();
        _cells.Clear();
    }

    public void PlayShuffleEffect()
    {
        const float duration = 0.1f;

        var shuffleTweenBuilder = GTweenSequenceBuilder.New();
        foreach (var humbleCell in _cells.Values)
        {
            var singleCellShuffle = humbleCell.TweenPosition(Vector2.Zero, duration)
                .SetEasing(Easing.OutQuint);
            shuffleTweenBuilder
                .Append(singleCellShuffle);
        }

        var tween = shuffleTweenBuilder.Build()
            .SetMaxLoops(ResetMode.PingPong);
        // todo: 想办法在 Win.OnExit 中终止庆祝动画更合理
        tween.PlayAsyncUntilNodeDestroy(this).Forget();
    }
}
