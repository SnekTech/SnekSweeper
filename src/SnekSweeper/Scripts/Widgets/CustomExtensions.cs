using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class CustomExtensions
{
    public static async Task FadeOutAsync(this CanvasItem target, float duration = 1,
        Action? onComplete = null)
    {
        var tween = GTweenSequenceBuilder.New()
            .Append(target.TweenModulateAlpha(0, duration))
            .Build();
        tween.SetEasing(Easing.InOutCubic).OnComplete(onComplete);

        await tween.PlayAsync(CancellationToken.None);
    }

    public static async Task FadeInAsync(this CanvasItem target, float duration = 1,
        Action? onComplete = null)
    {
        var tween = GTweenSequenceBuilder.New()
            .Append(target.TweenModulateAlpha(1, duration))
            .Build();
        tween.SetEasing(Easing.InOutCubic).OnComplete(onComplete);

        await tween.PlayAsync(CancellationToken.None);
    }
}