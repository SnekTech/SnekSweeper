namespace SnekSweeper.UI.Common.AmbientBackground;

/// <summary>
/// 内置背景主题预设。数值是从调好的 demo 场景搬过来的
/// (demo 场景将来会改为直接使用这些预设, 避免两处各存一份)。
/// </summary>
/// <remarks>
/// 约定: 所有预设的 SpinSpeed 同号(都为正), 避免两个主题之间过渡时转速经过 0 而"反向"。
/// </remarks>
public static class AmbientThemes
{
    /// <summary>主菜单: 大漩涡 + 丰富配色 + 稍快。</summary>
    public static readonly AmbientTheme Menu = new()
    {
        Base = new Color(0.02f, 0.05f, 0.09f),
        Main = new Color(0.07f, 0.38f, 0.68f),
        Accent = new Color(0.88f, 0.34f, 0.24f),
        BandScale = 0.076f,
        BandHardness = 0.40f,
        SwirlAmount = 0.32f,
        SpinSpeed = 0.45f,
        FlowAmount = 1.00f,
        FlowSpeed = 1.10f,
        Brightness = 1.00f,
    };

    /// <summary>关卡: 单色调(PICO-8 暗色系) + 硬边平涂 + 慢速。</summary>
    public static readonly AmbientTheme Level = new()
    {
        Base = new Color(0.372549f, 0.341176f, 0.309804f), // PICO-8 #5F574F (索引 5)
        Main = new Color(0.113725f, 0.168627f, 0.32549f),  // PICO-8 #1D2B53 (索引 1)
        Accent = new Color(0.494118f, 0.145098f, 0.32549f), // PICO-8 #7E2553 (索引 2)
        BandScale = 0.093f,
        BandHardness = 1.00f,
        SwirlAmount = 0.069f,
        SpinSpeed = 0.15f,
        FlowAmount = 0.987f,
        FlowSpeed = 0.55f,
        Brightness = 1.00f,
    };
}
