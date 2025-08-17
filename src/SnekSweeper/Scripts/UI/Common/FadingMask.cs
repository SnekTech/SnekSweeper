using GodotGadgets.Extensions;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common;

[SceneTree]
public partial class FadingMask : CanvasLayer, ISceneScript
{
    private const float FadingDuration = 0.3f;

    public Task FadeInAsync() => Panel.FadeInAsync(FadingDuration, this.GetCancellationTokenOnTreeExit());
    public Task FadeOutAsync() => Panel.FadeOutAsync(FadingDuration, this.GetCancellationTokenOnTreeExit());
}