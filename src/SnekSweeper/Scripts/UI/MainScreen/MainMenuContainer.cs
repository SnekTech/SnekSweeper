using GTweens.Builders;
using GTweens.Easings;
using GTweens.Extensions;
using GTweensGodot.Extensions;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.MainScreen;

[SceneTree]
public partial class MainMenuContainer : Control, ISceneScript
{
    const float SlideInDuration = 0.5f;
    public override void _Ready()
    {
        PlayMenuSlideInAnimation();
    }

    void PlayMenuSlideInAnimation()
    {
        var menu = _.MainMenu;
        var tweenAnchorTop = GTweenExtensions.Tween(() => menu.AnchorTop, x => menu.AnchorTop = x, 0.5f, SlideInDuration);
        var tweenAnchorBottom = GTweenExtensions.Tween(() => menu.AnchorBottom, x => menu.AnchorBottom = x, 0.5f, SlideInDuration);
        
        var tween = GTweenSequenceBuilder.New()
            .Join(tweenAnchorTop)
            .Join(tweenAnchorBottom)
            .Build();
        tween.SetEasing(Easing.OutBack);
        tween.Play();
    }
}