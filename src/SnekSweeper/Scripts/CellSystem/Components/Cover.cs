using System.Threading;
using System.Threading.Tasks;
using Godot;
using GTweensGodot.Extensions;

namespace SnekSweeper.CellSystem.Components;

public partial class Cover : Sprite2D, ICover
{
    private const float AnimationDuration = 0.2f;
    
    public async Task RevealAsync()
    {
        var tween = this.TweenScale(Vector2.Zero, AnimationDuration);
        await tween.PlayAsync(CancellationToken.None);
        Hide();
    }

    public async Task PutOnAsync()
    {
        Show();
        var tween = this.TweenScale(Vector2.One, AnimationDuration);
        await tween.PlayAsync(CancellationToken.None);
    }
}