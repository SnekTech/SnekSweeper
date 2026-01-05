using GodotGadgets.Tasks;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common;

[SceneTree]
public partial class FadingMask : CanvasLayer, ISceneScript
{
    private const float FadingDuration = 0.3f;

    public Task FadeInAsync(CancellationToken cancellationToken = default)
    {
        var linked = cancellationToken.LinkWithNodeDestroy(this);
        return Panel.FadeInAsync(FadingDuration, linked.Token);
    }

    public Task FadeOutAsync(CancellationToken cancellationToken = default)
    {
        var linked = cancellationToken.LinkWithNodeDestroy(this);
        return Panel.FadeOutAsync(FadingDuration, linked.Token);
    }
}