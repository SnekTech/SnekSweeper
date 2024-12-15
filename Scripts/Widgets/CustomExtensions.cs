using System.Threading.Tasks;
using Godot;

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
}