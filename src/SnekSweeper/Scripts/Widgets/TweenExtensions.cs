using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class TweenExtensions
{
    extension(CanvasItem target)
    {
        public Task FadeOutAsync(float duration, CancellationToken token)
            => target.TweenAlphaAsync(0, duration, token);

        public Task FadeInAsync(float duration, CancellationToken token)
        {
            target.Modulate = target.Modulate with { A = 0 };
            return target.TweenAlphaAsync(1, duration, token);
        }

        async Task TweenAlphaAsync(float to, float duration, CancellationToken token)
        {
            var tween = target.TweenModulateAlpha(to, duration)
                .SetEasing(Easing.InOutCubic);

            await tween.PlayAsync(token);
        }
    }
}