using GTweens.Easings;
using GTweens.Enums;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace SnekSweeper.UI.MainScreen;

public partial class PressToStartLabel : Label
{
    GTween? _blinkTween;
    const float BlinkDuration = 0.6f;

    public override void _Ready()
    {
        StartBlink();
    }

    public override void _ExitTree()
    {
        _blinkTween?.Kill();
    }

    void StartBlink()
    {
        if (_blinkTween is not null) return;

        _blinkTween = this.TweenModulateAlpha(0, BlinkDuration)
            .SetEasing(Easing.InOutSine)
            .SetMaxLoops(ResetMode.PingPong);
        _blinkTween.Play();
    }
}