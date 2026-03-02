using GTweens.Easings;
using GTweens.Enums;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace SnekSweeper.UI.MainScreen;

public partial class PressToStartLabel : Label
{
    public event Action? AnyKeyPressed;

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

    public override void _Input(InputEvent @event)
    {
        if (CanInputStartGame(@event))
        {
            AnyKeyPressed?.Invoke();
        }
        
        return;
        
        static bool CanInputStartGame(InputEvent inputEvent) =>
            inputEvent is InputEventMouseButton or InputEventKey or InputEventJoypadButton;
    }
}