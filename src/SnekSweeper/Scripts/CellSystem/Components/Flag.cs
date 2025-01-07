using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotUtilities;
using GTweens.Easings;
using GTweensGodot.Extensions;
using SnekSweeper.Constants;

namespace SnekSweeper.CellSystem.Components;

[Scene]
public partial class Flag : Node2D, IFlag
{
    [Node] private Sprite2D flagSprite = null!;

    private const float AnimationDuration = 0.2f;
    private const int StartPositionY = CoreStats.CellSizePixels;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        Hide();
        flagSprite.Position = flagSprite.Position with { Y = StartPositionY };
    }

    public async Task RaiseAsync()
    {
        Show();
        var tween = flagSprite.TweenPositionY(0, AnimationDuration).SetEasing(Easing.OutQuad);
        await tween.PlayAsync(CancellationToken.None);
    }

    public async Task PutDownAsync()
    {
        var tween = flagSprite.TweenPositionY(StartPositionY, AnimationDuration).SetEasing(Easing.InQuad);
        await tween.PlayAsync(CancellationToken.None);
        Hide();
    }
}