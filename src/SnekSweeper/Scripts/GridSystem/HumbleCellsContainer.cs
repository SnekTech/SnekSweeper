using GodotGadgets.Extensions;
using GodotGadgets.Tasks;
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

public partial class HumbleCellsContainer : Node2D, IHumbleCellsContainer
{
    public IHumbleCell InstantiateHumbleCell(GridIndex gridIndex, GridSkin gridSkin)
    {
        var humbleCell = HumbleCell.InstantiateOnParent(this);
        humbleCell.OnInstantiate(gridIndex, gridSkin);
        return humbleCell;
    }

    public IEnumerable<IHumbleCell> HumbleCells => this.GetChildrenOfType<HumbleCell>();

    public void Clear() => this.ClearChildren();

    public void PlayShuffleEffect()
    {
        const float duration = 0.1f;

        var shuffleTweenBuilder = GTweenSequenceBuilder.New();
        foreach (var humbleCell in this.GetChildrenOfType<HumbleCell>())
        {
            var singleCellShuffle = humbleCell.TweenPosition(Vector2.Zero, duration)
                .SetEasing(Easing.OutQuint);
            shuffleTweenBuilder
                .Append(singleCellShuffle);
        }

        var tween = shuffleTweenBuilder.Build()
            .SetMaxLoops(ResetMode.PingPong);
        tween.PlayAsync(this.GetCancellationTokenOnTreeExit()).Fire();
    }
}