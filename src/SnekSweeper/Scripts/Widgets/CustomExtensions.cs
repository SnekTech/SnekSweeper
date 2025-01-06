using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class CustomExtensions
{
    public static Task FadeOutAsync(this CanvasItem target, float duration = 1, Action? onComplete = null)
        => target.TweenAlphaAsync(0, duration, onComplete);

    public static Task FadeInAsync(this CanvasItem target, float duration = 1, Action? onComplete = null)
    {
        target.Modulate = target.Modulate with { A = 0 };
        return target.TweenAlphaAsync(1, duration, onComplete);
    }

    private static async Task TweenAlphaAsync(this CanvasItem target, float to, float duration, Action? onComplete)
    {
        var tween = target.TweenModulateAlpha(to, duration)
            .SetEasing(Easing.InOutCubic).OnComplete(onComplete);

        await tween.PlayAsync(CancellationToken.None);
    }
}