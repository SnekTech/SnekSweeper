using GodotGadgets.TweenStuff;
using GodotTask;
using GTweens.Easings;
using GTweens.Extensions;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common.AmbientBackground;

/// <summary>
/// 常驻的全屏背景。独立于当前场景 —— 场景切换时由外部调用 <see cref="GoTo"/> 平滑过渡到新主题。
/// </summary>
[SceneTree]
public partial class AmbientBackground : CanvasLayer, ISceneScript
{
    readonly CancellableTweenHolder _transition = new();

    AmbientPaintUniforms _uniforms = null!;

    public override void _Ready()
    {
        _uniforms = new AmbientPaintUniforms((ShaderMaterial)Paint.Material);

        // 铺底: 让材质持有显式值(不依赖"从未设置过的 uniform 读回什么"), 同时确定第一次过渡的起点。
        SnapTo(AmbientThemes.Menu);
    }

    public override void _ExitTree() => _transition.Dispose();

    /// <summary>立即切到某主题(不插值)。</summary>
    public void SnapTo(AmbientTheme theme) => _uniforms.Snap(theme);

    /// <summary>
    /// 平滑过渡到目标主题。上一次过渡若尚未结束会被取消, 且从"当前实际值"续接 —— 不会跳变。
    /// </summary>
    public GDTask GoToAsync(AmbientTheme theme, float duration, CancellationToken ct = default)
    {
        var from = _uniforms.Read();

        var progress = GTweenExtensions.Tween(
                () => 0f,                                    // 进度起点恒为 0; 真正的起点由 from 决定
                p => _uniforms.Snap(from.LerpTo(theme, p)),  // 每帧把插值结果写进材质
                1f,
                duration)
            .SetEasing(Easing.InOutCubic);

        return _transition.CancelPreviousAndPlayAsync(progress, ct).AsGDTask();
    }

    /// <summary>平滑过渡(不等待)。</summary>
    public void GoTo(AmbientTheme theme, float duration) => GoToAsync(theme, duration).Forget();
}
