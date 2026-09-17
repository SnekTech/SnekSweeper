using System.Threading.Tasks;
using GodotGadgets.ShaderStuff;
using GodotGadgets.Tasks;
using GodotGadgets.TweenStuff;
using GTweens.Extensions;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Cover : Node2D, ICover, ISceneScript
{
    const float AnimationDuration = .2f;

    Uniform<float> dissolveProgress = null!;
    Uniform<float> coverAlpha = null!;
    Uniform<float> noiseSeed = null!;

    // 动画取消走 CancellationToken：新动画开始前取消旧的，避免快速 Reveal/PutOn 交替互相打架
    CancellationTokenSource? _tweenCts;

    public override void _Ready()
    {
        var shaderMaterial = (ShaderMaterial)_.Sprite.Material;
        dissolveProgress = shaderMaterial.GetUniform<float>("progress");
        coverAlpha = shaderMaterial.GetUniform<float>("coverAlpha");
        noiseSeed = shaderMaterial.GetUniform<float>("noiseSeed");

        SetDissolveProgress(0);
    }

    public override void _ExitTree() => _tweenCts?.CancelAndDispose();

    public async Task RevealAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        RandomizeNoise();

        // 收尾动作挂在 OnComplete 上, 而不是写在 await 之后:
        //  - 自然完成时它同步执行(此刻节点必然还活着);
        //  - 被新动画取代、或节点销毁时 tween 会被 Kill, 而 Kill 不会触发 OnComplete。
        // 于是不存在"续体排在队列里、轮到它执行时节点已经死了"的窗口
        // (那个窗口会抛 ObjectDisposedException, 因为 await 的续体是被同步上下文在下一帧泵出来的)。
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 1, AnimationDuration)
            .OnComplete(Hide);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);

        // 取消（被新动画取代/节点销毁）时 await 抛 OCE；OCE 交给调用方的 fire-and-forget 处理
        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
    }

    public async Task PutOnAsync(CancellationToken ct = default)
    {
        _tweenCts?.CancelAndDispose();
        _tweenCts = new CancellationTokenSource();

        RandomizeNoise();
        Show();
        var tween = GTweenExtensions.Tween(GetDissolveProgress, SetDissolveProgress, 0, AnimationDuration);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _tweenCts.Token);
        await tween.PlayAsyncUntilNodeDestroy(this, linked.Token);
    }

    public void SetAlpha(float normalizedAlpha) => coverAlpha.Value = normalizedAlpha;

    public void SetStatus(CoverStatus status)
    {
        const float indicatorAlpha = 0.7f;

        StatusIndicator.Modulate = status switch
        {
            CoverStatus.Safe => Pico8Palette.Green with { A = indicatorAlpha },
            CoverStatus.Uncertain => Pico8Palette.Yellow with { A = indicatorAlpha },
            _ => Colors.Transparent,
        };
    }

    float GetDissolveProgress() => dissolveProgress.Value;
    void SetDissolveProgress(float progress) => dissolveProgress.Value = progress;

    void RandomizeNoise() => noiseSeed.Value = GD.Randf();
}
