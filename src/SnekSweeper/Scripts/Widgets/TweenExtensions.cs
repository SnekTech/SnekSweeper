using GodotGadgets.Tasks;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class TweenExtensions
{
    extension(CanvasItem target)
    {
        public Task FadeOutAsync(float duration, CancellationToken ct = default)
            => target.TweenAlphaAsync(0, duration, ct);

        public Task FadeInAsync(float duration, CancellationToken ct = default)
        {
            target.Modulate = target.Modulate with { A = 0 };
            return target.TweenAlphaAsync(1, duration, ct);
        }

        Task TweenAlphaAsync(float to, float duration, CancellationToken ct = default) =>
            target.TweenModulateAlpha(to, duration)
                .SetEasing(Easing.InOutCubic)
                .PlayAsyncUntilNodeDestroy(target, ct);
    }

    extension(Control target)
    {
        public async Task SlideInAsync(Vector2 destinationGlobal, float duration = 0.6f, CancellationToken ct = default)
        {
            target.Show();
            await target.TweenGlobalPosition(destinationGlobal, duration)
                .SetEasing(Easing.OutBack)
                .PlayAsyncUntilNodeDestroy(target, ct);
        }

        public async Task SlideOutAsync(Vector2 destinationGlobal, float duration = 0.6f,
            CancellationToken ct = default)
        {
            await target.TweenGlobalPosition(destinationGlobal, duration)
                .SetEasing(Easing.InBack)
                .PlayAsyncUntilNodeDestroy(target, ct);
            target.Hide();
        }
    }

    extension(GTween tween)
    {
        Task PlayAsyncUntilNodeDestroy(Node node, CancellationToken ct = default) =>
            tween.PlayAsync(ct.LinkWithNodeDestroy(node).Token);
    }
}