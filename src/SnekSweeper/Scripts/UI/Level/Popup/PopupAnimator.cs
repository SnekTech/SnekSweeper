using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Level.Popup;

public class PopupAnimator(Control parent)
{
    readonly Vector2 _originalGlobalPosition = parent.GlobalPosition;

    const float TweenDuration = 0.6f;

    public Task ShowAsync(Vector2 targetGlobalPosition, CancellationToken ct = default) =>
        parent.SlideInAsync(targetGlobalPosition, TweenDuration, ct);

    public Task HideAsync(CancellationToken ct = default) =>
        parent.SlideOutAsync(_originalGlobalPosition, TweenDuration, ct);
}