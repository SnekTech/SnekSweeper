using GodotGadgets.TooltipSystem;
using GTweensGodot.Extensions;

namespace SnekSweeper.UI.TooltipSystem;

[SceneTree]
public partial class Tooltip : Control
{
    const float FadeDuration = 0.3f;

    internal Task ShowAsync(TooltipContent content, Rect2 targetGlobalRect, CancellationToken token)
    {
        Header.Text = content.Title;
        Content.Text = content.Content;
        Show();
        
        Callable.From(UpdateTooltipPosition).CallDeferred();

        return this.TweenModulateAlpha(1, FadeDuration).PlayAsync(token);

        void UpdateTooltipPosition()
        {
            // the new tooltip panel container size can only
            // be obtained after the new tooltip appearing,
            // so the call to update the tooltip position should be deferred
            GlobalPosition = GetValidGlobalPosition(targetGlobalRect, _.PanelContainer.Get().Size, GetViewportRect().Size);
        }
    }

    internal async Task HideAsync(CancellationToken token)
    {
        await this.TweenModulateAlpha(0, FadeDuration).PlayAsync(token);
        Hide();
    }

    static Vector2 GetValidGlobalPosition(Rect2 targetGlobalRect, Vector2 tooltipSize, Vector2 viewportSize)
    {
        const int tooltipMarginX = 10;
        var targetPosition = targetGlobalRect.Position;
        var targetSize = targetGlobalRect.Size;
        
        var tooltipX = IsOverflowHorizontally()
            ? targetPosition.X - tooltipMarginX - tooltipSize.X
            : targetPosition.X + targetSize.X + tooltipMarginX;

        var tooltipY = IsOverflowVertically()
            ? targetPosition.Y - (tooltipSize.Y - targetSize.Y)
            : targetPosition.Y;

        return new Vector2(tooltipX, tooltipY);

        bool IsOverflowHorizontally() => targetPosition.X + tooltipMarginX + tooltipSize.X > viewportSize.X;
        bool IsOverflowVertically() => targetPosition.Y + tooltipSize.Y > viewportSize.Y;
    }
}