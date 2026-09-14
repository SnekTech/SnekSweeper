// ============================================================================
// AmbientPaint.gdshader
// ---------------------------------------------------------------------------
// 程序生成的"油彩"背景 —— 灵感源自 Balatro 的招牌背景动画。
// 结构脉络来自社区公开的 localthunk 血脉移植(参考 Godot Shaders "Balatro Paint Mix", CC0),
// 重写为适配本项目 640x360 像素风管线的 Godot 4 版本。
//
// 【设计】
//   只有一个【单中心大漩涡】, 菜单和关卡用的是同一个形态, 只是参数不同:
//     主菜单 = 大漩涡 + 丰富配色 + 稍快
//     关卡   = 同一个大漩涡 + 单色调 + 慢动画 + 更浅
//   因此所有参数都是连续的, 场景之间直接 lerp 这堆数字即可(不需要 fade in/out)。
//
// 【颜色: 按"角色"命名, 三段色带】
//   色带值 t 从 0 到 1, 依次经过:
//     colour_base   (t=0)   大面积底色
//     colour_main   (t=0.5) 主体色
//     colour_accent (t=1)   高光/强调色
//   输出【永远落在 base→main→accent 这条色带上】, 即任何时刻最多混合相邻两色,
//   不会混出"脏色"。两个控制:
//     band_hardness = 0 -> 三色之间绵软渐变
//     band_hardness = 1 -> 硬边平涂, 输出恰好等于你填的 3 个颜色
//                          (用严格调色板、PICO-8 这类配色时必须开大)
//     band_scale        -> 色带疏密: 越大亮色(accent)越多, 越小底色(base)越多
//
//   配色建议: 挑【依次过渡】的 3 个颜色(一条 ramp), 而不是 3 个互不相干的颜色。
//   单色调 = 3 个同色系、明度递进的颜色。
//   PICO-8 示例(sRGB hex):
//     关卡·暗蓝单色 : base #000000  main #1D2B53  accent #29ADFF
//     菜单·丰富     : base #1D2B53  main #FF004D  accent #FFEC27
//
// 【关于"像素化"这层 —— 重要】
//   本项目渲染管线: 先渲染到 640x360 低分辨率缓冲, 再整数倍放大到窗口
//   (project.godot: stretch/mode=viewport + scale_mode=integer + Nearest 过滤)。
//   因此整个屏幕(包括本 shader 输出)天然是等大的像素块, shader 每帧只在每个
//   game pixel 采样一次, 不需要为"像素大小一致 / 不错位"做任何事。
//   cell_px 只是【艺术性粗颗粒】旋钮(单位 = 游戏像素): 0=关, 2~4=油画平涂块感。
// ============================================================================

shader_type canvas_item;
render_mode unshaded;

// ---- 颜色(按角色命名) --------------------------------------------------------
uniform vec4 colour_base   : source_color = vec4(0.02, 0.05, 0.09, 1.0); // t=0   大面积底色
uniform vec4 colour_main   : source_color = vec4(0.07, 0.38, 0.68, 1.0); // t=0.5 主体色
uniform vec4 colour_accent : source_color = vec4(0.88, 0.34, 0.24, 1.0); // t=1   高光/强调色
uniform float band_scale    : hint_range(0.005, 0.20) = 0.05; // 色带疏密(越大亮色越多)
uniform float band_hardness : hint_range(0.0, 1.0) = 0.40;    // 0=绵软渐变 1=硬边平涂

// ---- 形态 -------------------------------------------------------------------
uniform float swirl_amount : hint_range(0.0, 1.0) = 0.32;  // 大漩涡深浅(0 = 不扭)
uniform float spin_speed   : hint_range(-2.0, 2.0) = 0.45; // 大漩涡自转速度

// ---- 流动 -------------------------------------------------------------------
uniform float flow_amount  : hint_range(0.0, 1.0) = 1.0;    // 搅拌强度
uniform float flow_speed   : hint_range(0.0, 4.0) = 1.1;    // 流动速度
uniform float field_zoom   : hint_range(1.0, 120.0) = 30.0; // 色场频率

// ---- 输出 -------------------------------------------------------------------
uniform float brightness : hint_range(0.0, 2.0) = 1.0;      // 整体亮度
uniform float cell_px    : hint_range(0.0, 8.0, 1.0) = 0.0; // 平涂颗粒(游戏像素, 0=关)

// ============================================================================
// 单中心大漩涡: 旋转量随"到中心的半径"增大
// ============================================================================
vec2 swirl(vec2 p) {
    float r = length(p);
    float a = atan(p.y, p.x)
            + TIME * spin_speed * 0.3
            - 20.0 * swirl_amount * r;
    return vec2(cos(a), sin(a)) * r;
}

// ============================================================================
// 流动: 5 次迭代域扭曲("颜料被搅拌")
// flow_amount 只缩放每次迭代的增量 => 0 = 恒等(只剩静态色带), 1 = 原版强度,
// 中间值连续可 lerp, 不会出现"坐标尺度突变"。
// ============================================================================
vec2 flow_warp(vec2 p) {
    p *= field_zoom;
    float t = TIME * flow_speed;
    vec2 q = vec2(p.x + p.y);
    for (int i = 0; i < 5; i++) {
        q += sin(max(p.x, p.y)) + p;
        p += flow_amount * 0.5 * vec2(cos(5.1123314 + 0.353 * q.y + t * 0.131121),
                                      sin(q.x - 0.113 * t));
        p -= flow_amount * (cos(p.x + p.y) - sin(p.x * 0.711 - p.y));
    }
    return p;
}

// ============================================================================
// 上色: base -> main -> accent 三段色带(最多混合相邻两色, 不会出脏色)
// ============================================================================
vec3 colorize(vec2 p) {
    // 线性标定: |p| 的实际量级约为 0 ~ field_zoom*0.5(屏幕上约 0~30),
    // band_scale≈0.035~0.05 正好把这段范围铺满整条色带 [0,1] => base->main->accent。
    // (注意: 大幅改动 field_zoom 时, 因 |p| 随之放大, 需要重新微调 band_scale。)
    float t = clamp(length(p) * band_scale, 0.0, 1.0);

    float seg = t * 2.0;          // 0..2, 三段的分界在 1.0
    float fr = fract(seg);        // 段内位置
    float w = mix(0.5, 0.02, band_hardness);       // 过渡带宽: 大=绵软, 小=硬边
    fr = smoothstep(0.5 - w, 0.5 + w, fr);

    vec3 lower = mix(colour_base.rgb, colour_main.rgb, fr);   // base <-> main
    vec3 upper = mix(colour_main.rgb, colour_accent.rgb, fr); // main <-> accent
    return mix(lower, upper, step(1.0, seg));
}

void fragment() {
    // 坐标准备: 以 viewport(game pixel)为单位, 居中并按对角线归一化
    vec2 res = 1.0 / SCREEN_PIXEL_SIZE;   // 本项目里 = (640, 360)
    vec2 px = FRAGCOORD.xy;
    if (cell_px > 0.0) {
        px = floor(px / cell_px) * cell_px;      // 可选: 游戏像素单位的粗颗粒
    }
    vec2 p = (px - 0.5 * res) / length(res);     // 居中, 范围约 [-0.44, 0.44]

    p = swirl(p);          // 单中心大漩涡
    p = flow_warp(p);      // 流动: "颜料被搅拌"
    COLOR = vec4(colorize(p) * brightness, 1.0);
}
