using System.Threading.Tasks;
using GodotGadgets.Tasks;
using GodotGadgets.TweenStuff;
using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Flag : Node2D, IFlag, ISceneScript
{
    const float AnimationDuration = .2f;
    const int StartPositionY = HumbleCell.CellSizeInPixels;

    // 动画取消走 CancellationToken：新动画开始前取消旧的，快速升降旗时取消进行中的动画
    CancellationTokenSource? _tweenCts;

    public override void _Ready()
    {
        Hide();
        FlagSprite.Position = FlagSprite.Position with { Y = StartPositionY };
    }

    public override void _ExitTree() => _tweenCts?.CancelAndDispose();

    public async Task RaiseAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        Show();
        var tween = FlagSprite.TweenPositionY(0, AnimationDuration).SetEasing(Easing.OutQuad);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);
        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
    }

    public async Task PutDownAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        var tween = FlagSprite.TweenPositionY(StartPositionY, AnimationDuration).SetEasing(Easing.InQuad);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);

        // 取消（被新动画取代/节点销毁）时 await 抛 OCE，自然跳过 Hide；OCE 交给调用方的 fire-and-forget 处理
        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
        Hide();
    }
}
