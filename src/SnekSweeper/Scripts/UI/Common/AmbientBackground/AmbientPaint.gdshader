// ============================================================================
// AmbientPaint.gdshader
// ---------------------------------------------------------------------------
// 程序生成的"油彩涡旋"背景 —— 灵感源自 Balatro 招牌背景动画。
// 结构脉络来自社区公开的 localthunk 血脉移植(参考 Godot Shaders "Balatro Paint Mix", CC0),
// 已重写为适配本项目 640x360 像素风管线的 Godot 4 版本。
//
// 【用法】
//   赋给一个铺满屏幕的 ColorRect 的 material 即可。纯程序生成, 不读任何纹理。
//
// 【关于"像素化"这层 —— 重要】
//   本项目的渲染管线是: 先渲染到 640x360 低分辨率缓冲, 再整体整数倍放大到窗口
//   (project.godot: stretch/mode=viewport + scale_mode=integer + Nearest 过滤)。
//   因此整个屏幕(包括本 shader 的输出)已经天然是等大的像素块 —— shader 每帧只
//   在每个 game pixel 采样一次, 不需要再为"像素大小一致 / 不错位"做任何事。
//
//   原版 Balatro shader 里的 floor(px/pixel_size) 那步, 在本管线里唯一剩下的价值是
//   一种【艺术性粗颗粒】("油画平涂的块状感")。于是这里收敛成一个旋钮 cell_px:
//     cell_px = 0   -> 关闭(默认): 靠 640x360 本身, 效果是"细腻的油彩";
//     cell_px = 2~4 -> 以【游戏像素】为单位做平涂块(1080p 下约等于 6~12 个屏幕像素),
//                      轴对齐在 game pixel 网格上, 整数放大后依然绝对对齐。
//
//   所有 uniform 都可在材质 Inspector 里实时调整, 用来当实验台。
// ============================================================================

shader_type canvas_item;
render_mode unshaded;

// ---- 调色板(3 个色带) -------------------------------------------------------
// 将来每场景/每主题换一套颜色 = 只换这 3 个 uniform,
// 天然支持"切换场景时丝滑 lerp 到下个主题色"。
uniform vec4 colour_1 : source_color = vec4(0.02, 0.05, 0.09, 1.0); // 深空底色带
uniform vec4 colour_2 : source_color = vec4(0.07, 0.38, 0.68, 1.0); // 主色(青蓝)
uniform vec4 colour_3 : source_color = vec4(0.88, 0.34, 0.24, 1.0); // 强调色(旗红)

// ---- 形态 / 运动 -------------------------------------------------------------
uniform float contrast     : hint_range(0.1, 8.0) = 2.6;   // 色带边界锐利度(勿设 0)
uniform float swirl_amount : hint_range(0.0, 1.0) = 0.32;  // 涡旋强度(0 = 不旋)
uniform float spin_speed   : hint_range(-3.0, 3.0) = 0.45; // 整体转速
uniform float flow_speed   : hint_range(0.0, 4.0) = 1.1;   // 颜料流动/搅拌速度
uniform float field_zoom   : hint_range(1.0, 120.0) = 30.0; // 色场频率
uniform float cell_px      : hint_range(0.0, 8.0, 1.0) = 0.0; // 平涂块大小(游戏像素, 0=关)
uniform float brightness   : hint_range(0.0, 2.0) = 1.0;   // 整体亮度

void fragment() {
    // --- 坐标准备: 以 viewport(game pixel)为单位, 居中并按对角线归一化 ---
    vec2 res = 1.0 / SCREEN_PIXEL_SIZE;   // 本项目里 = (640, 360)
    float diag = length(res);
    vec2 px = FRAGCOORD.xy;

    // 可选粗颗粒平涂(以游戏像素为单位、轴对齐; 0 = 关闭, 靠 640x360 本身)
    if (cell_px > 0.0) {
        px = floor(px / cell_px) * cell_px;
    }

    vec2 p = (px - 0.5 * res) / diag;     // 居中, 范围约 [-0.44, 0.44]
    float r0 = length(p);

    // --- 1) 极坐标涡旋: 旋转量随半径增大 => "旋转扭动" ---
    float angle = atan(p.y, p.x)
                + TIME * spin_speed * 0.3
                - 20.0 * (swirl_amount * r0 + (1.0 - swirl_amount));
    p = vec2(cos(angle), sin(angle)) * r0;

    // --- 2) 迭代域扭曲: "颜料被搅拌"(纯解析 cos/sin, 无噪声采样) ---
    p *= field_zoom;
    float t = TIME * flow_speed;
    vec2 q = vec2(p.x + p.y);
    for (int i = 0; i < 5; i++) {
        q += sin(max(p.x, p.y)) + p;
        p += 0.5 * vec2(cos(5.1123314 + 0.353 * q.y + t * 0.131121),
                        sin(q.x - 0.113 * t));
        p -= cos(p.x + p.y) - sin(p.x * 0.711 - p.y);
    }

    // --- 3) 距离量化成 3 个色带(海报化平色 = 油画调色盘的来源) ---
    float cm = 0.25 * contrast + 0.5 * swirl_amount + 1.2;
    float f = clamp(length(p) * 0.035 * cm, 0.0, 2.0);
    float c1 = max(0.0, 1.0 - cm * abs(1.0 - f));
    float c2 = max(0.0, 1.0 - cm * abs(f));
    float c3 = 1.0 - min(1.0, c1 + c2);

    vec3 base = colour_1.rgb;
    vec3 col = (0.3 / contrast) * base
             + (1.0 - 0.3 / contrast)
               * (base * c1 + colour_2.rgb * c2 + colour_3.rgb * c3);

    COLOR = vec4(col * brightness, 1.0);
}
