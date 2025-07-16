using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeper.Constants;
using SnekSweeper.Widgets;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Flag : Node2D, IFlag, ISceneScript
{
    private const float AnimationDuration = 0.2f;
    private const int StartPositionY = CoreStats.CellSizePixels;

    public override void _Ready()
    {
        Hide();
        FlagSprite.Position = FlagSprite.Position with { Y = StartPositionY };
    }

    public async Task RaiseAsync()
    {
        Show();
        var tween = FlagSprite.TweenPositionY(0, AnimationDuration).SetEasing(Easing.OutQuad);
        await tween.PlayAsync(CancellationToken.None);
    }

    public async Task PutDownAsync()
    {
        var tween = FlagSprite.TweenPositionY(StartPositionY, AnimationDuration).SetEasing(Easing.InQuad);
        await tween.PlayAsync(CancellationToken.None);
        Hide();
    }
}