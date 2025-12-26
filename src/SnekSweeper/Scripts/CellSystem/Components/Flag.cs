using GodotGadgets.Extensions;
using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeper.Constants;
using SnekSweeper.Widgets;
using SnekSweeperCore.CellSystem.Components;

namespace SnekSweeper.CellSystem.Components;

[SceneTree]
public partial class Flag : Node2D, IFlag, ISceneScript
{
    const float AnimationDuration = .2f;
    const int StartPositionY = CoreStats.CellSizePixels;

    public override void _Ready()
    {
        Hide();
        FlagSprite.Position = FlagSprite.Position with { Y = StartPositionY };
    }

    public async Task RaiseAsync(CancellationToken cancellationToken = default)
    {
        var tokenLinkedWithTreeExit = cancellationToken.LinkWithNodeDestroy(this);

        Show();
        var tween = FlagSprite.TweenPositionY(0, AnimationDuration).SetEasing(Easing.OutQuad);
        await tween.PlayAsync(tokenLinkedWithTreeExit.Token);
    }

    public async Task PutDownAsync(CancellationToken cancellationToken = default)
    {
        var tokenLinkedWithTreeExit = cancellationToken.LinkWithNodeDestroy(this);

        var tween = FlagSprite.TweenPositionY(StartPositionY, AnimationDuration).SetEasing(Easing.InQuad);
        await tween.PlayAsync(tokenLinkedWithTreeExit.Token);
        Hide();
    }
}