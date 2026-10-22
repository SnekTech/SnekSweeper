using System.Diagnostics;
using GodotGadgets.Tasks;
using GodotGadgets.TooltipSystem;
using GodotTask;

namespace SnekSweeper.UI.TooltipSystem;

[SceneTree]
public partial class TooltipLayer : CanvasLayer, ITooltipRenderer, ITooltipAttacher
{
    readonly TooltipHost _host;
    readonly LatestAction _fade = new();
    readonly TooltipConfig _config = TooltipConfig.Default;

    public TooltipLayer()
    {
        _host = new TooltipHost(_config, this, () => TimeSpan.FromMilliseconds(Time.GetTicksMsec()));
    }

    public override void _ExitTree()
    {
        _host.Stop();
        _fade.Dispose();
    }

    public override void _Process(double delta) => _host.Advance();

    public IDisposable Attach(TooltipSource source, TooltipContent content) => _host.Attach(source, content);

    public void Render(TooltipTransition transition)
    {
        switch (transition)
        {
            case TooltipTransition.Unchanged:
                return;

            case TooltipTransition.Show show:
                Tooltip.Present(show.View.Content, show.View.Anchor, _config.Margin);
                _fade.RunAsync(Tooltip.PlayFadeInAsync).Forget();
                break;

            case TooltipTransition.Replace replace:
                Tooltip.Present(replace.View.Content, replace.View.Anchor, _config.Margin);
                break;

            case TooltipTransition.Hide:
                _fade.RunAsync(Tooltip.FadeOutThenHideAsync).Forget();
                break;

            default: throw new UnreachableException();
        }
    }
}