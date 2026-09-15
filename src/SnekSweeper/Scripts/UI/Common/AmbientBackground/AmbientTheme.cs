namespace SnekSweeper.UI.Common.AmbientBackground;

/// <summary>
/// 一套背景视觉参数, 与 AmbientPaint.gdshader 里"可插值"的 uniform 一一对应。
/// </summary>
/// <remarks>
/// 不含 field_zoom / cell_px: 那两个是固定值, 由 shader 默认值决定, 不随主题变化。
/// 颜色是 Godot 类型, 因此本类型住在 Godot 工程内(不进 Core)。
/// </remarks>
public sealed record AmbientTheme
{
    /// <summary>shader: colour_base —— 色带 t=0, 大面积底色。</summary>
    public required Color Base { get; init; }

    /// <summary>shader: colour_main —— 色带 t=0.5, 主体色。</summary>
    public required Color Main { get; init; }

    /// <summary>shader: colour_accent —— 色带 t=1, 高光/强调色。</summary>
    public required Color Accent { get; init; }

    /// <summary>shader: band_scale —— 色带疏密(越大亮色越多)。</summary>
    public required float BandScale { get; init; }

    /// <summary>shader: band_hardness —— 0=绵软渐变, 1=硬边平涂(可用于严格调色板)。</summary>
    public required float BandHardness { get; init; }

    /// <summary>shader: swirl_amount —— 大漩涡深浅(0 = 不扭)。</summary>
    public required float SwirlAmount { get; init; }

    /// <summary>shader: spin_speed —— 自转速度, 带符号。约定所有预设同号, 避免过渡途中经过 0 反向。</summary>
    public required float SpinSpeed { get; init; }

    /// <summary>shader: flow_amount —— 搅拌强度。</summary>
    public required float FlowAmount { get; init; }

    /// <summary>shader: flow_speed —— 流动速度。</summary>
    public required float FlowSpeed { get; init; }

    /// <summary>shader: brightness —— 整体亮度。</summary>
    public required float Brightness { get; init; }
}

public static class AmbientThemeExtensions
{
    extension(AmbientTheme from)
    {
        /// <summary>在两个主题之间按 t(0..1) 插值。</summary>
        public AmbientTheme LerpTo(AmbientTheme to, float t) => new()
        {
            Base = from.Base.Lerp(to.Base, t),
            Main = from.Main.Lerp(to.Main, t),
            Accent = from.Accent.Lerp(to.Accent, t),
            BandScale = Mathf.Lerp(from.BandScale, to.BandScale, t),
            BandHardness = Mathf.Lerp(from.BandHardness, to.BandHardness, t),
            SwirlAmount = Mathf.Lerp(from.SwirlAmount, to.SwirlAmount, t),
            SpinSpeed = Mathf.Lerp(from.SpinSpeed, to.SpinSpeed, t),
            FlowAmount = Mathf.Lerp(from.FlowAmount, to.FlowAmount, t),
            FlowSpeed = Mathf.Lerp(from.FlowSpeed, to.FlowSpeed, t),
            Brightness = Mathf.Lerp(from.Brightness, to.Brightness, t),
        };
    }
}
