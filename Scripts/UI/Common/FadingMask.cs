using System.Threading.Tasks;
using Godot;
using GodotUtilities;

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

    public Task FadeInAsync() => FadeAsync(FadeIn);

    public Task FadeOutAsync() => FadeAsync(FadeOut);

    private Task FadeAsync(StringName animationName)
    {
        var tcs = new TaskCompletionSource();
        animationPlayer.Play(animationName);
        animationPlayer.AnimationFinished += OnAnimationFinished;

        return tcs.Task;

        void OnAnimationFinished(StringName _)
        {
            animationPlayer.AnimationFinished -= OnAnimationFinished;
            tcs.SetResult();
        }
    }

    private void ResetFadeAnimationLength(StringName animationName)
    {
        var animation = animationPlayer.GetAnimation(animationName);

        const int panelModulateTrackIndex = 0;
        const int lastKeyIndex = 1;
        animation.TrackSetKeyTime(panelModulateTrackIndex, lastKeyIndex, fadingTime);
        animation.Length = fadingTime;
    }
}