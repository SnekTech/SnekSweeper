using System.Runtime.CompilerServices;
using Dumpify;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace SnekSweeper.Widgets;

public static class CustomExtensions
{
    public static Task FadeOutAsync(this CanvasItem target, float duration = 1, Action? onComplete = null)
        => target.TweenAlphaAsync(0, duration, onComplete);

    public static Task FadeInAsync(this CanvasItem target, float duration = 1, Action? onComplete = null)
    {
        target.Modulate = target.Modulate with { A = 0 };
        return target.TweenAlphaAsync(1, duration, onComplete);
    }

    private static async Task TweenAlphaAsync(this CanvasItem target, float to, float duration, Action? onComplete)
    {
        var tween = target.TweenModulateAlpha(to, duration)
            .SetEasing(Easing.InOutCubic).OnComplete(onComplete);

        await tween.PlayAsync(CancellationToken.None);
    }

    public static void DumpGd<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))]
        string? autoLabel = null
    )
    {
        members = members is null ? new MembersConfig { IncludeFields = true } : null;

        GD.Print(obj.DumpText(
            label,
            maxDepth,
            renderer,
            useDescriptors,
            colors,
            members,
            typeNames,
            tableConfig,
            outputConfig,
            typeRenderingConfig,
            autoLabel
        ));
    }
}