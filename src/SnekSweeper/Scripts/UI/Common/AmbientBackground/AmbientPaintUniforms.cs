using GodotGadgets.ShaderStuff;

namespace SnekSweeper.UI.Common.AmbientBackground;

/// <summary>
/// 把一套 <see cref="AmbientTheme"/> 读/写到 AmbientPaint.gdshader 的材质上。
/// 只做执行、不做决策: 不知道场景类型, 也不管过渡时长与中断策略。
/// </summary>
/// <remarks>
/// 不包含 field_zoom / cell_px —— 那两个是固定值, 由 shader 默认值决定, 这里永不触碰。
/// </remarks>
sealed class AmbientPaintUniforms(ShaderMaterial material)
{
    const string BaseName = "colour_base";
    const string MainName = "colour_main";
    const string AccentName = "colour_accent";
    const string BandScaleName = "band_scale";
    const string BandHardnessName = "band_hardness";
    const string SwirlAmountName = "swirl_amount";
    const string SpinSpeedName = "spin_speed";
    const string FlowAmountName = "flow_amount";
    const string FlowSpeedName = "flow_speed";
    const string BrightnessName = "brightness";

    readonly Uniform<Color> _base = material.GetUniform<Color>(BaseName);
    readonly Uniform<Color> _main = material.GetUniform<Color>(MainName);
    readonly Uniform<Color> _accent = material.GetUniform<Color>(AccentName);
    readonly Uniform<float> _bandScale = material.GetUniform<float>(BandScaleName);
    readonly Uniform<float> _bandHardness = material.GetUniform<float>(BandHardnessName);
    readonly Uniform<float> _swirlAmount = material.GetUniform<float>(SwirlAmountName);
    readonly Uniform<float> _spinSpeed = material.GetUniform<float>(SpinSpeedName);
    readonly Uniform<float> _flowAmount = material.GetUniform<float>(FlowAmountName);
    readonly Uniform<float> _flowSpeed = material.GetUniform<float>(FlowSpeedName);
    readonly Uniform<float> _brightness = material.GetUniform<float>(BrightnessName);

    /// <summary>读出材质当前值(材质即唯一真相源)。</summary>
    public AmbientTheme Read() => new()
    {
        Base = _base.Value,
        Main = _main.Value,
        Accent = _accent.Value,
        BandScale = _bandScale.Value,
        BandHardness = _bandHardness.Value,
        SwirlAmount = _swirlAmount.Value,
        SpinSpeed = _spinSpeed.Value,
        FlowAmount = _flowAmount.Value,
        FlowSpeed = _flowSpeed.Value,
        Brightness = _brightness.Value,
    };

    /// <summary>立即写值, 不做过渡。用于初始化铺底。</summary>
    public void Snap(AmbientTheme theme)
    {
        _base.Value = theme.Base;
        _main.Value = theme.Main;
        _accent.Value = theme.Accent;
        _bandScale.Value = theme.BandScale;
        _bandHardness.Value = theme.BandHardness;
        _swirlAmount.Value = theme.SwirlAmount;
        _spinSpeed.Value = theme.SpinSpeed;
        _flowAmount.Value = theme.FlowAmount;
        _flowSpeed.Value = theme.FlowSpeed;
        _brightness.Value = theme.Brightness;
    }
}
