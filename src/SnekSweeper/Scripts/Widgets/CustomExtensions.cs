using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class CustomExtensions
{
    public static Task FadeOutAsync(this CanvasItem target, float duration, CancellationToken token)
        => target.TweenAlphaAsync(0, duration, token);

    public static Task FadeInAsync(this CanvasItem target, float duration, CancellationToken token)
    {
        target.Modulate = target.Modulate with { A = 0 };
        return target.TweenAlphaAsync(1, duration, token);
    }

    private static async Task TweenAlphaAsync(this CanvasItem target, float to, float duration, CancellationToken token)
    {
        var tween = target.TweenModulateAlpha(to, duration)
            .SetEasing(Easing.InOutCubic);

        await tween.PlayAsync(token);
    }
}