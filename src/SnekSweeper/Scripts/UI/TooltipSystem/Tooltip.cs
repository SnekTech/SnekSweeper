using GodotGadgets.Extensions;
using GodotGadgets.Tasks;
using GodotGadgets.TooltipSystem;
using GodotGadgets.TweenStuff;
using GodotTask;

namespace SnekSweeper.UI.TooltipSystem;

[SceneTree]
public partial class Tooltip : Control
{
    const float FadeDuration = 0.3f;

    public override void _Ready()
    {
        Hide();
        this.SetModulateAlpha(0);
    }

    internal async GDTask PlayFadeInAsync(CancellationToken token)
    {
        using var linked = token.LinkWithNodeDestroy(this);
        await this.FadeInAsync(FadeDuration, linked.Token);
    }

    internal async GDTask FadeOutThenHideAsync(CancellationToken token)
    {
        using var linked = token.LinkWithNodeDestroy(this);
        await this.FadeOutAsync(FadeDuration, linked.Token);

        linked.Token.ThrowIfCancellationRequested();
        Hide();
    }

    internal void Present(TooltipContent content, Rect2 anchor, float margin)
    {
        Apply(content);
        PlaceDeferred(anchor, margin);
    }

    void Apply(TooltipContent content)
    {
        HeaderLabel.Text = content.Title;
        ContentLabel.Text = content.Content;
        Show();
    }

    void PlaceDeferred(Rect2 anchor, float margin)
    {
        Callable.From(Place).CallDeferred();
        return;

        void Place()
        {
            // deferred 不可撤回，只能执行时兜底
            if (!IsInstanceValid(this)) return;

            // the new tooltip panel container size can only
            // be obtained after the new tooltip appearing,
            // so the call to update the tooltip position should be deferred
            GlobalPosition = TooltipPlacement.Resolve(
                anchor, _.PanelContainer.Get().Size, GetViewportRect().Size, margin);
        }
    }
}