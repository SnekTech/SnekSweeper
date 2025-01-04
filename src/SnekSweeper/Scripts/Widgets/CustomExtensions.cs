using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class CustomExtensions
{
    public static async Task PlayOneShotAsync(this AnimationPlayer animationPlayer, StringName animationName)
    {
        var animation = animationPlayer.GetAnimation(animationName);
        if (animation.LoopMode != Animation.LoopModeEnum.None)
        {
            GD.PushError($"animation {animationName} is looping, animation_finished signal will not be emitted");
            return;
        }

        animationPlayer.Play(animationName);
        await animationPlayer.ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }

    public static async Task FadeOutAsync(this CanvasItem target, float delay = 0, float duration = 1,
        Action? onComplete = null)
    {
        var tween = GTweenSequenceBuilder.New()
            .AppendTime(delay)
            .Append(target.TweenModulateAlpha(0, duration))
            .Build();
        tween.SetEasing(Easing.InOutCubic).OnComplete(onComplete);
        await tween.PlayAsync(CancellationToken.None);
    }
}