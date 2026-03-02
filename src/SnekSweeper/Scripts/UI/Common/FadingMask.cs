using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common;

[SceneTree]
public partial class FadingMask : CanvasLayer, ISceneScript
{
    const float FadingDuration = 0.3f;

    public Task FadeInAsync(CancellationToken ct = default) => Panel.FadeInAsync(FadingDuration, ct);

    public Task FadeOutAsync(CancellationToken ct = default) => Panel.FadeOutAsync(FadingDuration, ct);
}