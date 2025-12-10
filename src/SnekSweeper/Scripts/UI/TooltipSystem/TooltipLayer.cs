using GodotGadgets.Extensions;
using GodotGadgets.TooltipSystem;

namespace SnekSweeper.UI.TooltipSystem;

[SceneTree]
public partial class TooltipLayer : CanvasLayer, ITooltipDisplay
{
    static CancellationTokenSource _currentActionCancellationSource = new();

    public override void _Ready()
    {
        Tooltip.Hide();
    }

    public void ShowTooltip(TooltipContent content, Rect2 targetGlobalRect)
    {
        _currentActionCancellationSource.CancelAndDispose();
        _currentActionCancellationSource = new CancellationTokenSource();

        Tooltip.ShowAsync(content, targetGlobalRect, _currentActionCancellationSource.Token).Fire();
    }

    public void HideTooltip()
    {
        _currentActionCancellationSource.CancelAndDispose();
        _currentActionCancellationSource = new CancellationTokenSource();

        Tooltip.HideAsync(_currentActionCancellationSource.Token).Fire();
    }
}