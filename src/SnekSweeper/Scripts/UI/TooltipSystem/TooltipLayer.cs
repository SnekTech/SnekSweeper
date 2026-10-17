using System.Diagnostics;
using GodotGadgets.Tasks;
using GodotGadgets.TooltipSystem;
using GodotTask;

namespace SnekSweeper.UI.TooltipSystem;

[SceneTree]
public partial class TooltipLayer : CanvasLayer, ITooltipDisplay
{
    const float HoverDelaySeconds = 0.35f;

    TooltipSession _session = new TooltipSession.Hidden();
    CancellationTokenSource? _hoverDelay;
    readonly LatestAction _fade = new();
    RequestId _requestId;

    public override void _ExitTree()
    {
        _hoverDelay?.CancelAndDispose();
        _fade.Dispose();
    }

    public void PointerEntered(TooltipContent content, Rect2 targetGlobalRect)
    {
        var nextRequestId = _requestId.GetNext();
        _requestId = nextRequestId;
        Dispatch(new TooltipSession.Event.PointerEntered(new TooltipRequest(_requestId, content, targetGlobalRect)));
    }

    public void PointerExited()
    {
        Dispatch(new TooltipSession.Event.PointerExited());
    }

    void Dispatch(TooltipSession.Event evt)
    {
        if (!IsInsideTree()) return;
        
        var (next, effects) = _session.Update(evt, TimeSpan.FromSeconds(HoverDelaySeconds));
        _session = next;
        foreach (var effect in effects)
        {
            Apply(effect);
        }
    }

    void Apply(TooltipSession.Effect effect)
    {
        switch (effect)
        {
            case TooltipSession.Effect.StartHoverDelay { Delay: var delay, RequestId: var requestId }:
                _hoverDelay?.CancelAndDispose();
                _hoverDelay = new CancellationTokenSource();
                WaitThenDispatch(delay, requestId, _hoverDelay.Token).Forget();
                break;

            case TooltipSession.Effect.CancelHoverDelay:
                _hoverDelay?.CancelAndDispose();
                _hoverDelay = null;
                break;

            case TooltipSession.Effect.Show { Request: var request }:
                _hoverDelay?.CancelAndDispose();
                _hoverDelay = null;
                _fade.RunAsync(ct => Tooltip.ShowAsync(request.Content, request.TargetRect, ct)).Forget();
                break;

            case TooltipSession.Effect.FadeOut:
                _fade.RunAsync(Tooltip.HideAsync).Forget();
                break;

            default: throw new UnreachableException();
        }
    }

    async GDTask WaitThenDispatch(TimeSpan delay, RequestId requestId, CancellationToken ct)
    {
        await GDTask.Delay(delay, ct);
        ct.ThrowIfCancellationRequested();
        Dispatch(new TooltipSession.Event.HoverDelayElapsed(requestId));
    }
}