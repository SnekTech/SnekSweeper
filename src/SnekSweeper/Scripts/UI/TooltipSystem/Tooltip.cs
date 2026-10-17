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

    internal async GDTask ShowAsync(TooltipContent content, Rect2 targetGlobalRect, CancellationToken token)
    {
        using var linked = token.LinkWithNodeDestroy(this);

        HeaderLabel.Text = content.Title;
        ContentLabel.Text = content.Content;
        Show();

        Callable.From(UpdateTooltipPosition).CallDeferred();

        await this.FadeInAsync(FadeDuration, linked.Token);
        return;

        void UpdateTooltipPosition()
        {
            // deferred 不可撤回，只能执行时兜底
            if (!IsInstanceValid(this)) return;

            // the new tooltip panel container size can only
            // be obtained after the new tooltip appearing,
            // so the call to update the tooltip position should be deferred
            GlobalPosition = TooltipPlacement.Resolve(
                targetGlobalRect, _.PanelContainer.Get().Size, GetViewportRect().Size);
        }
    }

    internal async GDTask HideAsync(CancellationToken token)
    {
        using var linked = token.LinkWithNodeDestroy(this);
        await this.FadeOutAsync(FadeDuration, linked.Token);

        linked.Token.ThrowIfCancellationRequested();
        Hide();
    }
}