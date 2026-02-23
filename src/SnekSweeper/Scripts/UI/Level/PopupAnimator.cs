using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.UI.Level;

public class PopupAnimator(Control parent)
{
    readonly Vector2 _originalGlobalPosition = parent.GlobalPosition;

    const float TweenDuration = 0.6f;

    public async Task ShowAsync(Vector2 targetGlobalPosition, CancellationToken ct = default)
    {
        parent.Show();
        await parent.TweenGlobalPosition(targetGlobalPosition, TweenDuration)
            .SetEasing(Easing.OutBack)
            .PlayAsync(ct);
    }

    public async Task HideAsync(CancellationToken ct = default)
    {
        await parent.TweenGlobalPosition(_originalGlobalPosition, TweenDuration)
            .SetEasing(Easing.InBack)
            .PlayAsync(ct);
        parent.Hide();
    }
}