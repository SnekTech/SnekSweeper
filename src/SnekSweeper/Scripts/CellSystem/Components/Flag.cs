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
    const int RaisedPositionY = 0;
    const int PutDownPositionY = HumbleCell.CellSizeInPixels;

    // 动画取消走 CancellationToken：新动画开始前取消旧的，快速升降旗时取消进行中的动画
    CancellationTokenSource? _tweenCts;

    public override void _Ready()
    {
        SnapToPutDown();
        Hide();
    }

    public override void _ExitTree() => _tweenCts?.CancelAndDispose();

    void SnapToRaised() => FlagSprite.Position = FlagSprite.Position with { Y = RaisedPositionY };
    void SnapToPutDown() => FlagSprite.Position = FlagSprite.Position with { Y = PutDownPositionY };

    public void ShowRaised()
    {
        _tweenCts?.CancelAndDispose();
        SnapToRaised();
        Show();
    }

    public async Task RaiseAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        SnapToPutDown();
        Show();
        var tween = FlagSprite.TweenPositionY(RaisedPositionY, AnimationDuration).SetEasing(Easing.OutQuad);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);
        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
    }

    public async Task PutDownAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        SnapToRaised();
        var tween = FlagSprite.TweenPositionY(PutDownPositionY, AnimationDuration).SetEasing(Easing.InQuad)
            .OnComplete(Hide);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);

        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
    }
}