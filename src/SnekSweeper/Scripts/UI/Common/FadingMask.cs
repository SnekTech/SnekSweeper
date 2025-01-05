using System.Threading.Tasks;
using Godot;
using GodotUtilities;
using SnekSweeper.Widgets;

namespace SnekSweeper.UI.Common;

[Scene]
public partial class FadingMask : CanvasLayer
{
    [Node] private Panel panel = null!;

    private const float FadingTime = 0.3f;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public Task FadeInAsync() => panel.FadeInAsync(FadingTime);

    public Task FadeOutAsync() => panel.FadeOutAsync(FadingTime);
}