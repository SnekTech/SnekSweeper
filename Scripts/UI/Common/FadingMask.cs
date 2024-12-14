using System.Threading.Tasks;
using Godot;
using GodotUtilities;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common;

[Scene]
public partial class FadingMask : CanvasLayer
{
    private static readonly StringName FadeIn = "fade_in";
    private static readonly StringName FadeOut = "fade_out";

    [Export] private float fadingTime = 0.3f;

    [Node] private AnimationPlayer animationPlayer = null!;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        ResetFadeAnimationLength(FadeIn);
        ResetFadeAnimationLength(FadeOut);
    }

    public Task FadeInAsync() => animationPlayer.PlayOneShotAsync(FadeIn);

    public Task FadeOutAsync() => animationPlayer.PlayOneShotAsync(FadeOut);

    private void ResetFadeAnimationLength(StringName animationName)
    {
        var animation = animationPlayer.GetAnimation(animationName);

        const int panelModulateTrackIndex = 0;
        const int lastKeyIndex = 1;
        animation.TrackSetKeyTime(panelModulateTrackIndex, lastKeyIndex, fadingTime);
        animation.Length = fadingTime;
    }
}